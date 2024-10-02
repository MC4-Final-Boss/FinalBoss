using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;  

public class CameraController : MonoBehaviour
{
    Transform target;
    Vector3 velocity = Vector3.zero;

    [Range(0, 1)]
    public float smoothTime;
    public Vector3 cameraOffset;

    private void Awake() {
        StartCoroutine(FindPlayer());
    }

    IEnumerator FindPlayer()
    {
        // Check if this is the host or client
        bool isHost = NetworkManager.Singleton.IsHost;
        
        // Mencoba mencari player hingga ditemukan
        while (target == null)
        {
            GameObject player = null;

            if (isHost)
            {
                player = GameObject.FindGameObjectWithTag("Tanko");  // Assuming Tanko is tagged as "Host"
            }
            else
            {
                player = GameObject.FindGameObjectWithTag("Gaspi");  // Assuming Gaspi is tagged as "Client"
            }

            if (player != null)
            {
                target = player.transform;
            }

            // Tunggu satu frame sebelum mencoba lagi
            yield return null;
        }
    }

    private void LateUpdate() {
        if (target == null) return;

        Vector3 targetPosition = target.position + cameraOffset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}