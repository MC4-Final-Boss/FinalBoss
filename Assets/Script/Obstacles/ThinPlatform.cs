using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThinPlatform : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Tanko")){
            Destroy(gameObject, 0.5f);
        }
    }
} 