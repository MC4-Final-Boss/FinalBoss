using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Unity.Netcode;

public class RoomUIManager : MonoBehaviour
{
    [SerializeField] private Button createButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI roomCodeText;
    [SerializeField] private GameObject clientInputPanel;
    [SerializeField] private TMP_InputField roomCodeInput;
    [SerializeField] private Button joinButton;
    [SerializeField] private Button backButton;

    private string currentRoomCode;

    private void Start()
    {
        clientInputPanel.SetActive(false);
        statusText.gameObject.SetActive(false);
        joinButton.gameObject.SetActive(false);
        roomCodeText.gameObject.SetActive(false);

        createButton.onClick.AddListener(CreateRoom);
        clientButton.onClick.AddListener(ShowClientInputPanel);
        joinButton.onClick.AddListener(JoinRoom);
        backButton.onClick.AddListener(HandleBackButton);

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void CreateRoom()
    {
        Debug.Log("RoomUIManager: Create button clicked");
        createButton.gameObject.SetActive(false);
        clientButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
        statusText.gameObject.SetActive(true);
        statusText.text = "Creating room...";

        string localIP = GetLocalIPAddress();
        currentRoomCode = GenerateRoomCode(localIP);
        
        ShowRoomCode(currentRoomCode);
        NetworkManager.Singleton.StartHost();
        UpdateUI();
    }

    private void JoinRoom()
    {
        string inputCode = roomCodeInput.text;
        if (inputCode.Length != 6 || !int.TryParse(inputCode, out _))
        {
            statusText.text = "Invalid room code. Please try again.";
            return;
        }

        NetworkManager.Singleton.StartClient();
        UpdateUI();
    }

    private string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }

    private string GenerateRoomCode(string ipAddress)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(ipAddress + System.DateTime.Now.Ticks.ToString()));
            int hashInt = System.BitConverter.ToInt32(hashBytes, 0);
            return System.Math.Abs(hashInt % 1000000).ToString("D6");
        }
    }

    private void ShowRoomCode(string code)
    {
        roomCodeText.text = $"Room Code: {code}";
        roomCodeText.gameObject.SetActive(true);
        statusText.gameObject.SetActive(true);
        statusText.text = "Waiting for player to join...";
    }

    private void ShowClientInputPanel()
    {
        clientInputPanel.SetActive(true);
        joinButton.gameObject.SetActive(true);
        statusText.gameObject.SetActive(true);
        statusText.text = "Enter Room Code to join";
        createButton.gameObject.SetActive(false);
        clientButton.gameObject.SetActive(false);
    }

    private void UpdateUI()
    {
        createButton.gameObject.SetActive(!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost);
        clientButton.gameObject.SetActive(!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost);
        roomCodeText.gameObject.SetActive(NetworkManager.Singleton.IsHost);
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
            roomCodeText.gameObject.SetActive(false);
            joinButton.gameObject.SetActive(false);
        }
    }

    private void HandleBackButton()
    {
        if (clientInputPanel.activeSelf)
        {
            clientInputPanel.SetActive(false);
            createButton.gameObject.SetActive(true);
            clientButton.gameObject.SetActive(true);
            statusText.gameObject.SetActive(false);
        }
        else
        {
            SceneManager.LoadScene("IntroScene");
        }
    }

    private void OnClientConnected(ulong id)
    {
        Debug.Log($"RoomUIManager: Client connected. ID: {id}");
        UpdateUI();
        if (NetworkManager.Singleton.IsHost && NetworkManager.Singleton.ConnectedClientsList.Count > 1)
        {
            Debug.Log("RoomUIManager: All clients connected. Loading game scene 'Bustling City'.");
            LoadGameScene();
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