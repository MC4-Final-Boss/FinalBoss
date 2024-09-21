using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : NetworkBehaviour
{
    [SerializeField] private string gameSceneName = "BustlingCity";

    private NetworkVariable<int> playersInRoom = new NetworkVariable<int>();


    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
            NetworkManager.Singleton.SceneManager.OnLoadComplete += OnLoadComplete;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
            NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnLoadComplete;
        }
    }

    private void OnClientConnectedCallback(ulong clientId)
    {
        playersInRoom.Value++;
        Debug.Log($"RoomManager: Players in room: {playersInRoom.Value}");
        if (playersInRoom.Value == 2)
        {
            Debug.Log("RoomManager: All players joined, loading game scene");
            NetworkManager.Singleton.SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
        }
    }

    private void OnLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        Debug.Log($"RoomManager: Scene loaded. Scene: {sceneName}, Client: {clientId}");
        if (sceneName == gameSceneName && IsServer)
        {
            SpawnPlayers();
        }
    }

    private void SpawnPlayers()
    {
        Debug.Log("RoomManager: Spawning players");
        var clientIds = NetworkManager.Singleton.ConnectedClientsIds;
        int playerIndex = 0;
        foreach (var clientId in clientIds)
        {
            GameObject playerPrefab = (playerIndex == 0) ? 
                Resources.Load<GameObject>("Tanko") : 
                Resources.Load<GameObject>("Gaspi");
            GameObject playerInstance = Instantiate(playerPrefab);
            playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
            playerIndex++;
            Debug.Log($"RoomManager: Spawned player {playerIndex} for client {clientId}");
        }
    }
}