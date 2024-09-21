using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomUIManager : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private TextMeshProUGUI statusText;

    private void Start()
    {
        hostButton.onClick.AddListener(() => {
            Debug.Log("RoomUIManager: Host button clicked");
            NetworkManager.Singleton.StartHost();
            UpdateUI();
        });

        clientButton.onClick.AddListener(() => {
            Debug.Log("RoomUIManager: Client button clicked");
            NetworkManager.Singleton.StartClient();
            UpdateUI();
        });

        NetworkManager.Singleton.OnClientConnectedCallback += (id) => {
            Debug.Log($"RoomUIManager: Client connected callback. ID: {id}");
            UpdateUI();
        };
    }

    private void UpdateUI()
    {
        hostButton.gameObject.SetActive(!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost);
        clientButton.gameObject.SetActive(!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost);

        if (NetworkManager.Singleton.IsHost)
        {
            statusText.text = "Hosting. Waiting for player to join...";
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            statusText.text = "Joined. Waiting for game to start...";
        }
    }
}