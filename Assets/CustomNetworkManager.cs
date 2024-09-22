using Unity.Netcode;
using UnityEngine;

public class CustomNetworkManager : MonoBehaviour
{
    public GameObject tankoPrefab; // Prefab for Player 1
    public GameObject gaspiPrefab; // Prefab for Player 2

    private void Start()
{
    if (NetworkManager.Singleton != null)
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }
    else
    {
        Debug.LogError("NetworkManager not found. Make sure it is added to the scene.");
    }
}


    // private void OnDestroy()
    // {
    //     // Unregister when destroyed
    //     NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    // }

    private void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            // Determine which prefab to spawn based on whether the client is the server
            GameObject playerPrefab = clientId == NetworkManager.ServerClientId ? tankoPrefab : gaspiPrefab;

            // Spawn the correct player prefab
            GameObject playerInstance = Instantiate(playerPrefab, GetSpawnPosition(clientId), Quaternion.identity);

            // Make it a networked object and assign it to the client
            playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        }
    }

    private Vector3 GetSpawnPosition(ulong clientId)
    {
        // Customize spawn positions based on player
        if (clientId == NetworkManager.ServerClientId)
            return new Vector3(-2, 0, 0); // Position for Tanko (Player 1)
        else
            return new Vector3(2, 0, 0);  // Position for Gaspi (Player 2)
    }
}
