using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;

public class CreateAndJoin : MonoBehaviourPunCallbacks
{
    public TMP_InputField input_CreateJoin;
    public string gameSceneName = "SampleScene"; // Nama scene game Anda

    private void Start()
    {
        Debug.Log("Connecting to Photon...");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby successfully");
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(input_CreateJoin.text))
        {
            Debug.LogWarning("Room name is empty");
            return;
        }

        RoomOptions roomOptions = new RoomOptions { MaxPlayers = 2 };
        PhotonNetwork.CreateRoom(input_CreateJoin.text, roomOptions);
    }

    public void JoinRoom()
    {
        if (string.IsNullOrEmpty(input_CreateJoin.text))
        {
            Debug.LogWarning("Room name is empty");
            return;
        }

        PhotonNetwork.JoinRoom(input_CreateJoin.text);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room successfully.");

        // Load the game scene
        PhotonNetwork.LoadLevel(gameSceneName);
    }

    // public override void OnLevelLoaded(int levelNumber)
    // {
    //     if (levelNumber == PhotonNetwork.CurrentRoom.PlayerCount)
    //     {
    //         SpawnPlayer();
    //     }
    // }

    private void SpawnPlayer()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), 0);

        // Spawn Tanko if Player 1, Gaspi if Player 2
        GameObject player;
        if (PhotonNetwork.IsMasterClient)
        {
            // Player 1 becomes Tanko
            player = PhotonNetwork.Instantiate("Tanko", spawnPosition, Quaternion.identity);
        }
        else
        {
            // Player 2 becomes Gaspi
            player = PhotonNetwork.Instantiate("Gaspi", spawnPosition, Quaternion.identity);
        }

        if (player != null)
        {
            Debug.Log("Player instantiated at position: " + spawnPosition);
        }
        else
        {
            Debug.LogError("Player instantiation failed!");
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Create room failed: " + message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Join room failed: " + message);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError("Disconnected from Photon: " + cause);
        // Try to reconnect if desired
    }
}
