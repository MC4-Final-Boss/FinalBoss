using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private Vector3 respawnPosition;
    private PlayerSaveCheckPoint saveCheckpoint;

    private void Start()
    {
        saveCheckpoint = GetComponent<PlayerSaveCheckPoint>();

        if (saveCheckpoint == null)
        {
            Debug.LogError("PlayerSaveCheckPoint component is missing!");
        }
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
        // SaveCheckpoint();
    }

    private void SaveCheckpoint()
    {
        if (saveCheckpoint != null)
        {
            // saveCheckpoint.SaveCheckpoint(transform.position);
        }
    }
}
