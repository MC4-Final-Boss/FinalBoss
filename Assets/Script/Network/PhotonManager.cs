using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public string gameSceneName = "BustlingCity";
    private bool isPlayerSpawned = false;

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

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("New player entered the room. Total players: " + PhotonNetwork.CurrentRoom.PlayerCount);
        if (PhotonNetwork.IsMasterClient && SceneManager.GetActiveScene().name == gameSceneName)
        {
            StartCoroutine(SpawnPlayerWithDelay());
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name);
        if (scene.name == gameSceneName)
        {
            StartCoroutine(SpawnPlayerWithDelay());
        }
    }

    private IEnumerator SpawnPlayerWithDelay()
    {
        yield return new WaitForSeconds(1f); // Wait for 1 second to ensure all clients are ready
        if (!isPlayerSpawned)
        {
            SpawnPlayer();
            isPlayerSpawned = true;
        }
    }

    private void SpawnPlayer()
    {
        int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1; // ActorNumber starts from 1
        Vector3 spawnPosition = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), 0);
        string prefabName = (playerIndex % 2 == 0) ? "Tanko" : "Gaspi";
        
        Debug.Log("Attempting to spawn " + prefabName + " for player " + (playerIndex + 1) + " at position: " + spawnPosition);
        
        GameObject player = PhotonNetwork.Instantiate(prefabName, spawnPosition, Quaternion.identity);

        if (player != null)
        {
            Debug.Log(prefabName + " instantiated successfully for player " + (playerIndex + 1) + " at position: " + player.transform.position);
        }
        else
        {
            Debug.LogError(prefabName + " instantiation failed for player " + (playerIndex + 1) + "!");
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