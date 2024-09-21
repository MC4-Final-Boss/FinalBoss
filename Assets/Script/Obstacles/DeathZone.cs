using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Tanko") || other.gameObject.CompareTag("Gaspi"))
        {
            PlayerRespawn respawnScript = other.gameObject.GetComponent<PlayerRespawn>();
            
            if (respawnScript != null)
            {
                respawnScript.RespawnPlayer(); 
            }
        }
    }
}
