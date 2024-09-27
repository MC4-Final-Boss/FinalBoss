using UnityEngine;

public class PlayerSaveCheckPoint : MonoBehaviour
{
    public void SaveCheckpoint(Vector3 position)
    {
        PlayerPrefs.SetFloat("CheckpointX", transform.position.x);
        PlayerPrefs.SetFloat("CheckpointY", transform.position.y);
        PlayerPrefs.SetFloat("CheckpointZ", 0);
        PlayerPrefs.Save();
    }
}
