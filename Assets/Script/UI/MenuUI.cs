using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class RoomUIManager : MonoBehaviour
{
    [SerializeField] private Button createButton; // button create
    [SerializeField] private Button clientButton; // button join pertama
    [SerializeField] private TextMeshProUGUI statusText; // status waiting
    [SerializeField] private TextMeshProUGUI relayCodeText; // kode relay yang ditampilkan
    [SerializeField] private GameObject clientInputPanel; // UI Panel for client input
    [SerializeField] private TMP_InputField relayCodeInput; // client input kode relay
    [SerializeField] private Button joinButton; // join dengan kode relay
    [SerializeField] private Button backButton; // back button

    private void Start()
    {
        // Hide client input panel and status text initially
        clientInputPanel.SetActive(false);
        statusText.gameObject.SetActive(false);
        joinButton.gameObject.SetActive(false);
        relayCodeText.gameObject.SetActive(false);

        // Create button functionality
        createButton.onClick.AddListener(async () => {
            Debug.Log("RoomUIManager: Create button clicked");
            createButton.gameObject.SetActive(false);
            clientButton.gameObject.SetActive(false);
            backButton.gameObject.SetActive(false);
            statusText.gameObject.SetActive(true);
            statusText.text = "Loading";
            string relayCode = await RelayManager.Instance.CreateRelay();
            if (relayCode != null)
            {
                ShowRelayCode(relayCode);
                NetworkManager.Singleton.StartHost();
                UpdateUI();
            }
            else
            {
                Debug.LogError("Failed to create relay");
            }
        });

        // Client button functionality
        clientButton.onClick.AddListener(() => {
            Debug.Log("RoomUIManager: Client button clicked");
            ShowClientInputPanel();
        });

        // Join button functionality
        joinButton.onClick.AddListener(async () => {
            bool joinSuccess = await RelayManager.Instance.JoinRelay(relayCodeInput.text);
            if (joinSuccess)
            {
                NetworkManager.Singleton.StartClient();
                UpdateUI();
            }
            else
            {   
                statusText.gameObject.SetActive(true);
                statusText.text = "Code Mismatch, try again";
                Debug.LogError("Failed to join relay");
                ShowClientInputPanel();
            }
        });

        // Back button functionality
        backButton.onClick.AddListener(() => {
            if (clientInputPanel.activeSelf)
            {
                // Close client input panel and show create & client buttons
                clientInputPanel.SetActive(false);
                createButton.gameObject.SetActive(true);
                clientButton.gameObject.SetActive(true);
                statusText.gameObject.SetActive(false); // Hide status text when returning
            }
            else
            {
                // Go back to 'IntroScene'
                SceneManager.LoadScene("IntroScene");
            }
        });

        // Client connected callback
        NetworkManager.Singleton.OnClientConnectedCallback += (id) => {
            Debug.Log($"RoomUIManager: Client connected callback. ID: {id}");
            UpdateUI();
            if (NetworkManager.Singleton.IsHost && NetworkManager.Singleton.ConnectedClientsList.Count > 1)
            {
                Debug.Log("RoomUIManager: All clients connected. Loading game scene 'Bustling City'.");
                LoadGameScene();
            }
        };
    }

    private void ShowRelayCode(string code)
    {
        relayCodeText.text = $"Code: {code}";
        statusText.gameObject.SetActive(true);
        statusText.text = "Waiting for player to join...";
    }

    private void ShowClientInputPanel()
    {
        clientInputPanel.SetActive(true); // Show client input panel
        joinButton.gameObject.SetActive(true);
        statusText.gameObject.SetActive(true);
        statusText.text = "Enter Code to join";
        createButton.gameObject.SetActive(false);
        clientButton.gameObject.SetActive(false);
    }

    private void UpdateUI()
    {
        createButton.gameObject.SetActive(!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost);
        clientButton.gameObject.SetActive(!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost);
        relayCodeText.gameObject.SetActive(true);
        statusText.gameObject.SetActive(true);

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
            NetworkManager.Singleton.SceneManager.LoadScene("BustlingCityScene", LoadSceneMode.Single);
        }
    }
}
