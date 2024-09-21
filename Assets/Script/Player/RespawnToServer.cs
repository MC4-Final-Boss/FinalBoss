using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections;

public class RespawnToServer : MonoBehaviourPunCallbacks
{
    public GameObject tankoPrefab;
    public GameObject gaspiPrefab;
    public Transform[] initialSpawnPoints;
    public float respawnDelay = 3f;

    private GameObject localPlayerInstance;
    private Vector3 respawnPosition;

    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        else if (PhotonNetwork.IsConnected && !PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        if (PhotonNetwork.InRoom)
        {
            SpawnPlayer();
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        LoadCheckpoint();
        GameObject playerPrefab = (PhotonNetwork.IsMasterClient) ? tankoPrefab : gaspiPrefab;
        localPlayerInstance = PhotonNetwork.Instantiate(playerPrefab.name, respawnPosition, Quaternion.identity);
    }

    private void LoadCheckpoint()
    {
        if (PlayerPrefs.HasKey("CheckpointX"))
        {
            float x = PlayerPrefs.GetFloat("CheckpointX");
            float y = PlayerPrefs.GetFloat("CheckpointY");
            float z = PlayerPrefs.GetFloat("CheckpointZ");
            respawnPosition = new Vector3(x, y, z);
        }
        else
        {
            int spawnIndex = PhotonNetwork.IsMasterClient ? 0 : 1;
            respawnPosition = initialSpawnPoints[spawnIndex % initialSpawnPoints.Length].position;
        }
    }

    public void SaveCheckpoint(Vector3 position)
    {
        PlayerPrefs.SetFloat("CheckpointX", position.x);
        PlayerPrefs.SetFloat("CheckpointY", position.y);
        PlayerPrefs.SetFloat("CheckpointZ", position.z);
        PlayerPrefs.Save();
    }

    public void OnPlayerDeath()
    {
        if (localPlayerInstance != null && localPlayerInstance.GetComponent<PhotonView>().IsMine)
        {
            StartCoroutine(RespawnPlayerCoroutine());
        }
    }

    private IEnumerator RespawnPlayerCoroutine()
    {
        if (localPlayerInstance != null)
        {
            PhotonNetwork.Destroy(localPlayerInstance);
        }

        yield return new WaitForSeconds(respawnDelay);

        RespawnPlayer();
    }

    public void RespawnPlayer()
    {
        LoadCheckpoint();
        GameObject playerPrefab = (PhotonNetwork.IsMasterClient) ? tankoPrefab : gaspiPrefab;
        localPlayerInstance = PhotonNetwork.Instantiate(playerPrefab.name, respawnPosition, Quaternion.identity);
        Debug.Log("Player respawned at: " + respawnPosition);
    }

    public void ForceRespawn()
    {
        if (localPlayerInstance != null && localPlayerInstance.GetComponent<PhotonView>().IsMine)
        {
            StartCoroutine(RespawnPlayerCoroutine());
        }
        else
        {
            SpawnPlayer();
        }
    }
}