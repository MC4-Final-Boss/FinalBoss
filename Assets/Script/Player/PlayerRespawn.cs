using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private Vector3 respawnPosition;

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

    public void SaveCheckpoint(Vector3 position)
    {
        PlayerPrefs.SetFloat("CheckpointX", position.x);
        PlayerPrefs.SetFloat("CheckpointY", position.y);
        PlayerPrefs.SetFloat("CheckpointZ", position.z);
        PlayerPrefs.Save();
    }

    public void RespawnPlayer()
    {
        LoadCheckpoint();
        transform.position = respawnPosition;
    }
}
