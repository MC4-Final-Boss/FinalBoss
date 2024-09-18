using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
     private void OnCollisionEnter2D(Collision2D other)
    {
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
