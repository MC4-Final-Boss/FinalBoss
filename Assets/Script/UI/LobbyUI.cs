using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class RoomUIManager : MonoBehaviour
{
    [SerializeField] private Button createButton; //button create
    [SerializeField] private Button clientButton; //button join pertama
    [SerializeField] private Button exitButton; // button exit
    [SerializeField] private TextMeshProUGUI statusText; //status waiting
    [SerializeField] private GameObject relayCodePanel; //gameObject script relay manager
    [SerializeField] private TextMeshProUGUI relayCodeText; //kode relay yang ditampilkan
    [SerializeField] private GameObject clientInputPanel; //??
    [SerializeField] private TMP_InputField relayCodeInput; //client input kode relay
    [SerializeField] private Button joinButton; //join dengan kode relay

    private void Start()
    {
        // Hide panels and status text initially
        // relayCodePanel.SetActive(false);
        clientInputPanel.SetActive(false);
        statusText.gameObject.SetActive(false);
        relayCodeText.gameObject.SetActive(false);
        relayCodeInput.gameObject.SetActive(false);
        joinButton.gameObject.SetActive(false);

        createButton.onClick.AddListener(async () => {
            Debug.Log("RoomUIManager: Create button clicked");
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

        clientButton.onClick.AddListener(() => {
            Debug.Log("RoomUIManager: Client button clicked");
            ShowClientInputPanel();
        });

        joinButton.onClick.AddListener(async () => {
            bool joinSuccess = await RelayManager.Instance.JoinRelay(relayCodeInput.text);
            if (joinSuccess)
            {
                NetworkManager.Singleton.StartClient();
                UpdateUI();
            }
            else
            {
                Debug.LogError("Failed to join relay");
            }
        });

        NetworkManager.Singleton.OnClientConnectedCallback += (id) => {
            Debug.Log($"RoomUIManager: Client connected callback. ID: {id}");
            UpdateUI();
            if (NetworkManager.Singleton.IsHost && NetworkManager.Singleton.ConnectedClientsList.Count > 1)
            {
                Debug.Log("RoomUIManager: All clients connected. Loading game scene 'Theme1'.");
                LoadGameScene();
            }
        };
    }

    private void ShowRelayCode(string code)
    {
        
        relayCodePanel.SetActive(true);
        relayCodeText.text = $"Relay Code: {code}";
        statusText.gameObject.SetActive(true);
        statusText.text = "Hosting. Waiting for player to join...";
    }

    private void ShowClientInputPanel()
    {
        clientInputPanel.SetActive(true);
        statusText.gameObject.SetActive(true);
        statusText.text = "Enter Relay Code to join";
        createButton.gameObject.SetActive(false);
        clientButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);
        relayCodeInput.gameObject.SetActive(true);
        joinButton.gameObject.SetActive(true);
    }

    private void UpdateUI()
    {
        createButton.gameObject.SetActive(!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost);
        clientButton.gameObject.SetActive(!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost);
        exitButton.gameObject.SetActive(false);
        relayCodeText.gameObject.SetActive(true);
        statusText.gameObject.SetActive(true);

        if (NetworkManager.Singleton.IsHost)
        {
            statusText.text = "Hosting. Waiting for player to join...";
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            statusText.text = "Joined. Waiting for game to start...";
            clientInputPanel.SetActive(false);
            relayCodeText.gameObject.SetActive(false);
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