using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerNetworkManager : NetworkBehaviour
{
    public GameObject tankoPrefab;
    public GameObject gaspiPrefab;
    public bool isStarted = false;

    [SerializeField] private float interpolationBackTime = 0.1f; // 100ms, adjust as needed
    [SerializeField] private float timeSyncInterval = 1f; // Sync every second

    private NetworkVariable<double> m_NetworkTime = new NetworkVariable<double>();
    private double m_LastServerTimeSent;
    private double m_ClientTimeOffset;

    // Variable to track connection status
    private bool wasConnected = false;

    // Reference to the DialogDisconnect script
    [SerializeField] private DialogDisconnect dialogDisconnect;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Debug.Log($"OnNetworkSpawn called. IsHost: {IsHost}, IsServer: {IsServer}, IsClient: {IsClient}");
        
        if (IsHost)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SpawnAndSetupPlayer;
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
            m_NetworkTime.Value = NetworkManager.ServerTime.Time;
            Debug.Log("Registered callbacks in OnNetworkSpawn");
        }

        // Set initial connection status
        wasConnected = NetworkManager.Singleton.IsConnectedClient;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (IsHost)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SpawnAndSetupPlayer;
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
            Debug.Log("Unregistered callbacks in OnNetworkDespawn");
        }
    }

    private void OnClientConnectedCallback(ulong clientId)
    {
        Debug.Log($"Client {clientId} connected");
    }

    private void OnClientDisconnectCallback(ulong clientId)
    {
        Debug.Log($"Client {clientId} disconnected");

        if (NetworkManager.Singleton.IsHost)
        {
            // Check if there are any clients left
            if (NetworkManager.Singleton.ConnectedClients.Count <= 1) // Only the host is left
            {
                Debug.Log("All clients have disconnected, shutting down the room.");
                ShutdownRoom();
            }
            else
            {
                // Host mengirim sinyal ke semua client untuk kembali ke menuScene
                NotifyAllClientsToCloseGameClientRpc("A player has disconnected, returning to main menu.");
            }
        }
        else if (clientId == NetworkManager.LocalClientId)
        {
            // Untuk client yang terputus, tampilkan alert dan kembali ke menuScene
            dialogDisconnect.ShowDisconnectAlert("You have been disconnected from the server.");
            // Optionally return to main menu for the client
            CloseGameOnDisconnect();
        }
    }

    // Method to handle room shutdown
    private void ShutdownRoom()
    {
        Debug.Log("Shutting down the room and returning to the main menu.");
        NotifyAllClientsToCloseGameClientRpc("The room is closing as all clients have disconnected.");
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("RizuMenuScene");
    }


    private void SpawnAndSetupPlayer(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clients, List<ulong> clientsTimedOut)
    {
        Debug.Log($"SpawnAndSetupPlayer called. Clients: {string.Join(", ", clients)}, TimedOut: {string.Join(", ", clientsTimedOut)}");

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!NetworkManager.Singleton.ConnectedClients.ContainsKey(clientId))
            {
                Debug.LogWarning($"Client {clientId} is not in ConnectedClients dictionary. Skipping spawn.");
                continue;
            }

            try
            {
                // Tentukan posisi spawn berdasarkan apakah client adalah host atau bukan
                var spawnPosition = GetSpawnPositionForPlayer(clientId == NetworkManager.ServerClientId);
                
                // Tentukan prefab yang akan digunakan untuk pemain
                GameObject playerPrefab = clientId == NetworkManager.ServerClientId ? tankoPrefab : gaspiPrefab;
                
                // Tetapkan tag berdasarkan apakah pemain adalah host (Tanko) atau client (Gaspi)
                string tagToAssign = clientId == NetworkManager.ServerClientId ? "Tanko" : "Gaspi";

                // Instantiate prefab di posisi spawn
                GameObject playerInstance = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
                playerInstance.tag = tagToAssign;

                // Pastikan objek memiliki komponen NetworkObject untuk dikendalikan oleh NetworkManager
                NetworkObject networkObject = playerInstance.GetComponent<NetworkObject>();
                if (networkObject == null)
                {
                    Debug.LogError($"NetworkObject not found on player prefab for client {clientId}!");
                    continue;
                }

                // Spawn player object di server dan klien
                networkObject.SpawnAsPlayerObject(clientId, true);
                Debug.Log($"Spawned player object for client {clientId} at position {spawnPosition}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error spawning player object for client {clientId}: {e.Message}");
            }
        }

        // Mulai permainan setelah pemain di-spawn
        StartGameClientRpc();
    }

    private Vector3 GetSpawnPositionForPlayer(bool isHost)
    {
        // Kembalikan posisi spawn berdasarkan apakah pemain adalah host
        return isHost ? new Vector3(-5, 0, 0) : new Vector3(-4, 0, 0);
    }

    [ClientRpc]
    private void StartGameClientRpc()
    {
        if (!IsClient) return;
        Debug.Log("StartGameClientRpc called on client");
        isStarted = true;
    }

    private void Update()
    {
        if (NetworkManager.Singleton != null)
        {
            // Periksa apakah client terputus tiba-tiba
            if (wasConnected && !NetworkManager.Singleton.IsConnectedClient)
            {
                Debug.LogWarning("Client disconnected unexpectedly!");
                dialogDisconnect.ShowDisconnectAlert("You have been disconnected from the server.");
                CloseGameOnDisconnect();
                wasConnected = false;
            }
            else if (!wasConnected && NetworkManager.Singleton.IsConnectedClient)
            {
                wasConnected = true;
            }

            if (IsServer)
            {
                UpdateServerTime();
            }
            else
            {
                UpdateClientTime();
            }
        }
    }

    private void UpdateServerTime()
    {
        if (NetworkManager.ServerTime.Time - m_LastServerTimeSent > timeSyncInterval)
        {
            m_NetworkTime.Value = NetworkManager.ServerTime.Time;
            m_LastServerTimeSent = NetworkManager.ServerTime.Time;
        }
    }

    private void UpdateClientTime()
    {
        m_ClientTimeOffset = m_NetworkTime.Value - NetworkManager.LocalTime.Time + interpolationBackTime;
    }

    public double GetNetworkTime()
    {
        return IsServer ? NetworkManager.ServerTime.Time : NetworkManager.LocalTime.Time + m_ClientTimeOffset;
    }

    public double GetInterpolatedServerTime()
    {
        return GetNetworkTime() - interpolationBackTime;
    }

        // Method untuk menutup game dan kembali ke main menu saat disconnect
    private void CloseGameOnDisconnect()
    {
        Debug.Log("Closing game and returning to main menu due to disconnection.");
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("RizuMenuScene");
    }

    // ClientRpc untuk memberitahu semua client untuk menutup game
    [ClientRpc]
    private void NotifyAllClientsToCloseGameClientRpc(string message)
    {
        dialogDisconnect.ShowDisconnectAlert(message);
    }
}
