using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomNetworkManagerWithTag : NetworkBehaviour
{
    public GameObject tankoPrefab;
    public GameObject gaspiPrefab;
    public bool isStarted = false;

    [SerializeField] private float interpolationBackTime = 0.1f; // 100ms, adjust as needed
    [SerializeField] private float timeSyncInterval = 1f; // Sync every second

    private NetworkVariable<double> m_NetworkTime = new NetworkVariable<double>();
    private double m_LastServerTimeSent;
    private double m_ClientTimeOffset;

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
        // You might want to delay spawning until the client is fully ready
        // Or use this to trigger a specific "ready" message from the client
    }

    private void OnClientDisconnectCallback(ulong clientId)
    {
        Debug.Log($"Client {clientId} disconnected");
        // Handle client disconnection (e.g., remove their objects, update game state)
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
                var spawnPosition = GetSpawnPositionForPlayer(clientId == NetworkManager.ServerClientId);
                GameObject playerPrefab = clientId == NetworkManager.ServerClientId ? tankoPrefab : gaspiPrefab;
                string tagToAssign = clientId == NetworkManager.ServerClientId ? "Tanko" : "Gaspi";

                GameObject playerInstance = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
                playerInstance.tag = tagToAssign;

                NetworkObject networkObject = playerInstance.GetComponent<NetworkObject>();
                if (networkObject == null)
                {
                    Debug.LogError($"NetworkObject not found on player prefab for client {clientId}!");
                    continue;
                }

                networkObject.SpawnAsPlayerObject(clientId, true);
                Debug.Log($"Spawned player object for client {clientId} at position {spawnPosition}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error spawning player object for client {clientId}: {e.Message}");
            }
        }

        StartGameClientRpc();
    }

    private Vector3 GetSpawnPositionForPlayer(bool isHost)
    {
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
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsConnectedClient)
        {
            Debug.LogWarning("Client disconnected unexpectedly!");
            // Handle unexpected disconnection (e.g., show reconnect UI, cleanup)
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
        // Recalculate client time offset
        m_ClientTimeOffset = m_NetworkTime.Value - NetworkManager.LocalTime.Time + interpolationBackTime;
    }

    // Use this method to get the current network time for gameplay logic
    public double GetNetworkTime()
    {
        if (IsServer)
        {
            return NetworkManager.ServerTime.Time;
        }
        else
        {
            return NetworkManager.LocalTime.Time + m_ClientTimeOffset;
        }
    }

    // Use this to get the server time on clients, accounting for interpolation
    public double GetInterpolatedServerTime()
    {
        return GetNetworkTime() - interpolationBackTime;
    }
}