using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomNetworkManagerWithTag : NetworkBehaviour
{
    public GameObject tankoPrefab;
    public GameObject gaspiPrefab;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Debug.Log("NETWROK SPAWN get called");
        if (IsHost) NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SpawnAndSetupPlayer;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (IsHost) NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SpawnAndSetupPlayer;
    }

    // private void OnClientConnected(ulong clientId)
    // {
    //     if (NetworkManager.Singleton.IsServer)
    //     {
    //         SpawnAndSetupPlayer(clientId);
    //     }
    // }

    private void SpawnAndSetupPlayer(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clients, List<ulong> clientsTimedOut)
    {
        Debug.Log("Invoked.....");
        GameObject playerPrefab;
        string tagToAssign;
        //bool isHost = NetworkManager.Singleton.IsHost &&  == NetworkManager.Singleton.LocalClientId;

        // if (IsServer)
        // {
        //     // Host (Player 1) gets Tanko
        //     playerPrefab = tankoPrefab;
        //     tagToAssign = "Tanko";
        // }
        // else
        // {
        //     // Client (Player 2) gets Gaspi
        //     playerPrefab = gaspiPrefab;
        //     tagToAssign = "Gaspi";
        // }

        var spawnPosition = GetSpawnPositionForPlayer(IsServer);
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            if (clientId == NetworkManager.ServerClientId) {
                // Host (Player 1) gets Tanko
                playerPrefab = tankoPrefab;
                tagToAssign = "Tanko";
            } else {
                // Client (Player 2) gets Gaspi
                playerPrefab = gaspiPrefab;
                tagToAssign = "Gaspi";
            }
            GameObject playerInstance = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
            playerInstance.tag = tagToAssign;
            // Pastikan objek memiliki komponen NetworkObject
            NetworkObject networkObject = playerInstance.GetComponent<NetworkObject>();
            if (networkObject == null)
            {
                Debug.LogError("NetworkObject tidak ditemukan pada prefab pemain!");
                return;
            }

            networkObject.SpawnAsPlayerObject(clientId, true);
            SetupObserved(playerInstance);
        }
        
        
        
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