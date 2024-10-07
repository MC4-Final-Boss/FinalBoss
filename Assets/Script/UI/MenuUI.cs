using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class RoomUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button createButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI relayCodeText;
    [SerializeField] private GameObject hostCodePanel;
    [SerializeField] private GameObject clientInputPanel;
    [SerializeField] private TMP_InputField relayCodeInput;
    [SerializeField] private Button joinButton;
    [SerializeField] private Button backButton;

    [Header("Settings")]
    [SerializeField] private string menuSceneName = "RizuIntroScene";
    [SerializeField] private string gameSceneName = "NewBustlingCityScene";
    [SerializeField] private float connectionTimeout = 30f;

    private bool isConnecting = false;
    private float connectionTimer = 0f;

    private void Start()
    {
        InitializeUI();
        SetupButtonListeners();
        SetupNetworkCallbacks();
        PlayerPrefs.DeleteAll();
    }

    private void Update()
    {
        if (isConnecting)
        {
            connectionTimer += Time.deltaTime;
            if (connectionTimer >= connectionTimeout)
            {
                HandleConnectionTimeout();
            }
        }
    }

    private void InitializeUI()
    {
        clientInputPanel.SetActive(false);
        hostCodePanel.SetActive(false);
        statusText.gameObject.SetActive(false);
        joinButton.gameObject.SetActive(false);
        relayCodeText.gameObject.SetActive(false);
    }

    private void SetupButtonListeners()
    {
        createButton.onClick.AddListener(OnCreateButtonClicked);
        clientButton.onClick.AddListener(OnClientButtonClicked);
        joinButton.onClick.AddListener(OnJoinButtonClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);
    }

    private void SetupNetworkCallbacks()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager is null! Ensure it exists in the scene.");
            return;
        }

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private async void OnCreateButtonClicked()
    {
        Debug.Log("RoomUIManager: Create button clicked");
        SetActiveMainButtons(false);
        ShowLoadingStatus();
        StartConnectionTimer();

        string relayCode = await RelayManager.Instance.CreateRelay();
        if (relayCode != null)
        {
            hostCodePanel.SetActive(true);
            ShowRelayCode(relayCode);
            NetworkManager.Singleton.StartHost();
            UpdateUI();
        }
        else
        {
            StopConnectionTimer();
            Debug.LogError("Failed to create relay");
            SetActiveMainButtons(true);
            ShowErrorStatus("Failed to create room");
        }
    }

    private void OnClientButtonClicked()
    {
        Debug.Log("RoomUIManager: Client button clicked");
        ShowClientInputPanel();
    }

    private async void OnJoinButtonClicked()
    {
        if (string.IsNullOrEmpty(relayCodeInput.text))
        {
            ShowErrorStatus("Please enter a code");
            return;
        }

        ShowLoadingStatus();
        StartConnectionTimer();

        bool joinSuccess = await RelayManager.Instance.JoinRelay(relayCodeInput.text);
        if (joinSuccess)
        {
            NetworkManager.Singleton.StartClient();
            UpdateUI();
        }
        else
        {
            StopConnectionTimer();
            ShowErrorStatus("Code mismatch, try again!");
        }
    }

    private void OnBackButtonClicked()
    {
        if (clientInputPanel.activeSelf || hostCodePanel.activeSelf)
        {
            ResetToInitialState();
        }
        else
        {
            LoadMenuScene();
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"RoomUIManager: Client connected callback. ID: {clientId}");
        StopConnectionTimer();
        UpdateUI();
        
        if (NetworkManager.Singleton.IsHost && NetworkManager.Singleton.ConnectedClientsList.Count > 1)
        {
            Debug.Log($"RoomUIManager: All clients connected. Loading game scene '{gameSceneName}'.");
            LoadGameScene();
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"Client disconnected: {clientId}");
        StopConnectionTimer();
        
        if (DialogDisconnect.Instance != null)
        {
            DialogDisconnect.Instance.ShowDisconnectAlert("You have been disconnected from the server.");
        }
        else
        {
            Debug.LogError("DialogDisconnect instance not found!");
            ResetToInitialState();
        }
    }

    private void HandleConnectionTimeout()
    {
        Debug.Log("Connection attempt timed out");
        StopConnectionTimer();
        ShowErrorStatus("Connection timed out. Please try again.");
        ResetToInitialState();
    }

    private void StartConnectionTimer()
    {
        isConnecting = true;
        connectionTimer = 0f;
    }

    private void StopConnectionTimer()
    {
        isConnecting = false;
        connectionTimer = 0f;
    }

    private void ShowRelayCode(string code)
    {
        relayCodeText.text = $"Code: {code}";
        statusText.gameObject.SetActive(true);
        statusText.text = "Waiting for player to join...";
    }

    private void ShowClientInputPanel()
    {
        clientInputPanel.SetActive(true);
        joinButton.gameObject.SetActive(true);
        statusText.gameObject.SetActive(true);
        statusText.text = "Enter Code to join";
        SetActiveMainButtons(false);
    }

    private void UpdateUI()
    {
        bool isConnected = NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsHost;
        SetActiveMainButtons(!isConnected);
        
        relayCodeText.gameObject.SetActive(NetworkManager.Singleton.IsHost);
        statusText.gameObject.SetActive(true);
        statusText.color = Color.white;

        if (NetworkManager.Singleton.IsHost)
        {
            statusText.text = "Hosting.\nWaiting for player to join...";
            clientInputPanel.SetActive(false);
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            statusText.text = "Joined. Waiting for game to start...";
            clientInputPanel.SetActive(false);
            relayCodeText.gameObject.SetActive(false);
            joinButton.gameObject.SetActive(false);
        }
    }

    private void LoadGameScene()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
        }
    }

    private void LoadMenuScene()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene(menuSceneName);
    }

    private void SetActiveMainButtons(bool active)
    {
        if (createButton != null) createButton.gameObject.SetActive(active);
        if (clientButton != null) clientButton.gameObject.SetActive(active);
    }

    private void ShowLoadingStatus()
    {
        if (statusText != null)
        {
            statusText.gameObject.SetActive(true);
            statusText.text = "Loading...";
            statusText.color = Color.white;
        }
    }

    private void ShowErrorStatus(string message)
    {
        if (statusText != null)
        {
            statusText.gameObject.SetActive(true);
            statusText.text = message;
            statusText.color = Color.red;
        }
    }

    private void ResetToInitialState()
    {
        StopConnectionTimer();
        
        if (clientInputPanel != null) clientInputPanel.SetActive(false);
        if (hostCodePanel != null) hostCodePanel.SetActive(false);
        if (relayCodeText != null) relayCodeText.gameObject.SetActive(false);
        if (joinButton != null) joinButton.gameObject.SetActive(false);
        
        SetActiveMainButtons(true);
        
        if (statusText != null)
        {
            statusText.gameObject.SetActive(false);
            statusText.color = Color.white;
        }
        
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }
}