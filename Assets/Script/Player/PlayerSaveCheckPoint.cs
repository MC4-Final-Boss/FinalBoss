using UnityEngine;

public class PlayerSaveCheckPoint : MonoBehaviour
{
    // Method to save the checkpoint position using PlayerPrefs
    public void SaveCheckpoint(Vector3 position)
    {
        PlayerPrefs.SetFloat("CheckpointX", position.x);
        PlayerPrefs.SetFloat("CheckpointY", position.y);
        PlayerPrefs.SetFloat("CheckpointZ", position.z);
        PlayerPrefs.Save(); // Save changes to PlayerPrefs
        Debug.Log("Checkpoint saved at: " + position);
    }

    // Method to retrieve the saved checkpoint position
    public Vector3 GetCheckpointPosition()
    {
        // Check if the checkpoint values are stored in PlayerPrefs
        float x = PlayerPrefs.GetFloat("CheckpointX", 0f); // Default to 0 if not found
        float y = PlayerPrefs.GetFloat("CheckpointY", 0f); // Default to 0 if not found
        float z = PlayerPrefs.GetFloat("CheckpointZ", 0f); // Default to 0 if not found
        
        Vector3 checkpointPosition = new Vector3(x, y, z);
        
        // Log the retrieved checkpoint position
        Debug.Log("Checkpoint retrieved: " + checkpointPosition);
        return checkpointPosition;
    }

    // Optional: Method to clear the saved checkpoint
    public void ClearCheckpoint()
    {
        PlayerPrefs.DeleteKey("CheckpointX");
        PlayerPrefs.DeleteKey("CheckpointY");
        PlayerPrefs.DeleteKey("CheckpointZ");
        PlayerPrefs.Save(); // Save changes to PlayerPrefs
        Debug.Log("Checkpoint cleared.");
    }
}
