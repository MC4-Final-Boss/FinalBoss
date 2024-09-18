using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private Vector3 respawnPosition;

    void Start()
    {
        LoadCheckpoint();  
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
            respawnPosition = transform.position;
        }
    }

    public void RespawnPlayer()
    {
        LoadCheckpoint();
        transform.position = respawnPosition;
        Debug.Log("Player respawned at: " + respawnPosition);
    }
}
