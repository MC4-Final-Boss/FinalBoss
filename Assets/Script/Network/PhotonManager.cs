using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room successfully.");

        Vector3 spawnPosition = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), 0);

        // Spawn Tanko jika Player 1, Gaspi jika Player 2
        GameObject player;
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1) 
        {
            // Player 1 menjadi Tanko
            player = PhotonNetwork.Instantiate("Tanko", spawnPosition, Quaternion.identity);
        } 
        else 
        {
            // Player 2 menjadi Gaspi
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

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server");
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning("Disconnected from server. Cause: " + cause.ToString());
    }
}
