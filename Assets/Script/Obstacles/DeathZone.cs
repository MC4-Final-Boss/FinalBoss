using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
     private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Tanko") || other.gameObject.CompareTag("Gaspi"))
        {
            GaspiController respawnScript = other.gameObject.GetComponent<GaspiController>();
            
            // if (respawnScript != null)
            // {
            //     respawnScript.RespawnPlayer(); 
            // }
        }
    }
}
