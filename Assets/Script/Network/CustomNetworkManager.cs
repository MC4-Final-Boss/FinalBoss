using Unity.Netcode;
using UnityEngine;

public class CustomNetworkManagerWithTag : MonoBehaviour
{
    public GameObject tankoPrefab;
    public GameObject gaspiPrefab;

    private void OnEnable()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnDisable()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            SpawnAndSetupPlayer(clientId);
        }
    }

    private void SpawnAndSetupPlayer(ulong clientId)
    {
        GameObject playerPrefab;
        string tagToAssign;
        bool isHost = NetworkManager.Singleton.IsHost && clientId == NetworkManager.Singleton.LocalClientId;

        if (isHost)
        {
            // Host (Player 1) gets Tanko
            playerPrefab = tankoPrefab;
            tagToAssign = "Tanko";
        }
        else
        {
            // Client (Player 2) gets Gaspi
            playerPrefab = gaspiPrefab;
            tagToAssign = "Gaspi";
        }

        var spawnPosition = GetSpawnPositionForPlayer(isHost);
        GameObject playerInstance = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        playerInstance.tag = tagToAssign;

        // Pastikan objek memiliki komponen NetworkObject
        NetworkObject networkObject = playerInstance.GetComponent<NetworkObject>();
        if (networkObject == null)
        {
            Debug.LogError("NetworkObject tidak ditemukan pada prefab pemain!");
            return;
        }

        networkObject.SpawnAsPlayerObject(clientId);
        SetupObserved(playerInstance);
    }

    private Vector3 GetSpawnPositionForPlayer(bool isHost)
    {
        return isHost ? new Vector3(-5, 0, 0) : new Vector3(5, 0, 0);
    }

    private void SetupObserved(GameObject playerInstance)
    {
        if (playerInstance.CompareTag("Tanko"))
        {
            playerInstance.AddComponent<Player1Observed>();
        }
        else if (playerInstance.CompareTag("Gaspi"))
        {
            playerInstance.AddComponent<Player2Observed>();
        }
    }
}