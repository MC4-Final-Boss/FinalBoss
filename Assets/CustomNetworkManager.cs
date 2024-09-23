using Unity.Netcode;
using UnityEngine;

public class CustomNetworkManagerWithTag : MonoBehaviour
{
    public GameObject tankoPrefab;
    public GameObject gaspiPrefab;

    private void OnEnable()
    {
        // Subscribe to the OnClientConnectedCallback event
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnDisable()
    {
        // Unsubscribe from the event when the object is disabled or destroyed
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            AssignPlayerPrefab(clientId);
        }
    }

    private void AssignPlayerPrefab(ulong clientId)
    {
        GameObject playerPrefab;
        string tagToAssign;

        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            // Player 1 gets Tanko
            playerPrefab = tankoPrefab;
            tagToAssign = "Tanko";
        }
        else
        {
            // Player 2 gets Gaspi
            playerPrefab = gaspiPrefab;
            tagToAssign = "Gaspi";
        }

        var spawnPosition = GetSpawnPositionForPlayer(clientId);
        GameObject playerInstance = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);

        // Assign tag to the player instance
        playerInstance.tag = tagToAssign;

        // Spawn as a networked object
        playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);

        // Start observing (syncing) the player instance for network updates
        SetupObserved(playerInstance);
    }

    private Vector3 GetSpawnPositionForPlayer(ulong clientId)
    {
        // Custom spawn positions
        return clientId == 0 ? new Vector3(-5, 0, 0) : new Vector3(5, 0, 0);
    }

    private void SetupObserved(GameObject playerInstance)
    {
        // Add observed script or set up properties to observe
        if (playerInstance.tag == "Tanko")
        {
            // Set up observing for Tanko (Player 1)
            playerInstance.AddComponent<Player1Observed>(); 
        }
        else if (playerInstance.tag == "Gaspi")
        {
            // Set up observing for Gaspi (Player 2)
            playerInstance.AddComponent<Player2Observed>(); 
        }
    }
}
