using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class CreateAndJoin : MonoBehaviourPunCallbacks
{
    public TMP_InputField input_CreateJoin;
   
    private bool isConnectedToMaster = false;

    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server");
        isConnectedToMaster = true;
    }

    public void CreateRoom()
    {
        if (!isConnectedToMaster)
        {
            Debug.LogError("Not connected to Master Server. Can't create room.");
            return;
        }

        if (string.IsNullOrEmpty(input_CreateJoin.text))
        {
            Debug.LogWarning("Room name is empty");
            return;
        }
        PhotonNetwork.CreateRoom(input_CreateJoin.text);
    }

    public void JoinRoom()
    {
        if (!isConnectedToMaster)
        {
            Debug.LogError("Not connected to Master Server. Can't join room.");
            return;
        }
        PhotonNetwork.JoinRoom(input_CreateJoin.text);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room successfully");
        PhotonNetwork.LoadLevel("SampleScene");
    }
}
