using UnityEngine;

public class PlayerSaveCheckPoint : MonoBehaviour
{
    Vector3 checkpointPosition;


    public void SaveCheckpoint(Vector3 position)
    {


        PlayerPrefs.SetFloat("CheckpointX", position.x);
        PlayerPrefs.SetFloat("CheckpointY", position.y);
        PlayerPrefs.SetFloat("CheckpointZ", position.z);
        PlayerPrefs.Save(); 
        // Debug.Log("Checkpoint saved at: " + position);
    }

    public Vector3 GetCheckpointPosition()
    {

        float x = PlayerPrefs.GetFloat("CheckpointX", 0f); // Default to 0 if not found
        float y = PlayerPrefs.GetFloat("CheckpointY", 0f); // Default to 0 if not found
        float z = PlayerPrefs.GetFloat("CheckpointZ", 0f); // Default to 0 if not found
        
        //Debug.Log("Checkpoint retrieved: " + checkpointPosition);
        return checkpointPosition;
    }

    public void ClearCheckpoint()
    {
        PlayerPrefs.DeleteKey("CheckpointX");
        PlayerPrefs.DeleteKey("CheckpointY");
        PlayerPrefs.DeleteKey("CheckpointZ");
        PlayerPrefs.Save(); // Save changes to PlayerPrefs
        //Debug.Log("Checkpoint cleared.");
    }
}