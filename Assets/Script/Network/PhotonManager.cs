using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public string gameSceneName = "SampleScene";

    void Start()
    {
        Debug.Log("PhotonManager Start");
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Connecting to Photon...");
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.Log("Already connected to Photon");
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server");
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room successfully. Players in room: " + PhotonNetwork.CurrentRoom.PlayerCount);
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("I am Master Client. Loading level: " + gameSceneName);
            PhotonNetwork.LoadLevel(gameSceneName);
        }
        else
        {
            Debug.Log("Waiting for Master Client to load the level");
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning("Disconnected from server. Cause: " + cause.ToString());
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("New player entered the room. Total players: " + PhotonNetwork.CurrentRoom.PlayerCount);
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("I am Master Client. Spawning players for all");
            SpawnPlayersForAll();
        }
    }

    private void SpawnPlayersForAll()
    {
        Debug.Log("Spawning players for all. Players in room: " + PhotonNetwork.CurrentRoom.PlayerCount);
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            SpawnPlayer(i);
        }
    }

    private void SpawnPlayer(int playerIndex)
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), 0);
        string prefabName = (playerIndex == 0) ? "Tanko" : "Gaspi";
        
        Debug.Log("Attempting to spawn " + prefabName + " at position: " + spawnPosition);
        
        GameObject player = PhotonNetwork.Instantiate(prefabName, spawnPosition, Quaternion.identity);

        if (player != null)
        {
            Debug.Log(prefabName + " instantiated successfully at position: " + player.transform.position);
        }
        else
        {
            Debug.LogError(prefabName + " instantiation failed!");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name);
        if (scene.name == gameSceneName)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("I am Master Client. Spawning players after scene load");
                SpawnPlayersForAll();
            }
            else
            {
                Debug.Log("I am not Master Client. Waiting for Master Client to spawn players");
            }
        }
    }

    private void OnEnable()
    {
        Debug.Log("PhotonManager OnEnable");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        Debug.Log("PhotonManager OnDisable");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}