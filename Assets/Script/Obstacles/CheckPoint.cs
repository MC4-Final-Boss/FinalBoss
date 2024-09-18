using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool hasTriggered;

    private void Start() {
        hasTriggered = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Tanko") || other.gameObject.CompareTag("Gaspi"))
        {
            PlayerPrefs.SetFloat("CheckpointX", transform.position.x);
            PlayerPrefs.SetFloat("CheckpointY", transform.position.y);
            PlayerPrefs.SetFloat("CheckpointZ", 0);  

            PlayerPrefs.Save();  
            Debug.Log("Checkpoint saved at: " + new Vector3(transform.position.x, transform.position.y, 0));
            
            hasTriggered = true; 
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {       
        if (other.gameObject.CompareTag("Tanko"))
        {
            hasTriggered = false;  
        }
    }
}
