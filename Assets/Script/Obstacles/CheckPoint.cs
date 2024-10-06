using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // private bool hasTriggered;
    public PlayerSaveCheckPoint saveCheckpoint;


    private void Start()
    {
        // hasTriggered = false;
        // saveCheckpoint = GetComponent<PlayerSaveCheckPoint>();

        if (saveCheckpoint == null)
        {
            //Debug.LogError("PlayerSaveCheckPoint component is missing!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Tanko") || other.gameObject.CompareTag("Gaspi"))
        {

            SaveCheckpoint();
            //Debug.Log("Checkpoint saved at: " + new Vector3(transform.position.x, transform.position.y, 0));
            // hasTriggered = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Tanko"))
        {
            // hasTriggered = false;
        }
    }

    private void SaveCheckpoint()
    {
        if (saveCheckpoint != null)
        {
            // saveCheckpoint.SaveCheckpoint(transform.position);
            saveCheckpoint.ClearCheckpoint();
        }
    }
}
