using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThinPlatform : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        // if (other.gameObject.CompareTag("Tanko") || other.gameObject.CompareTag("Gaspi")){
        //     Destroy(gameObject, 0.5f);
        // }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.CompareTag("Tanko") || other.gameObject.CompareTag("Gaspi")){
            Destroy(gameObject, 0.4f);
        }
    }
} 