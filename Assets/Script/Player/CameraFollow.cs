
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Transform target;

    Vector3 velocity = Vector3.zero;

    [Range(0,1)]
    public float smoothTime;

    public Vector3 cameraOfset;

    private void Awake() {
        StartCoroutine(FindPlayer());
    }

    IEnumerator FindPlayer()
    {
        // Mencoba mencari player hingga ditemukan
        while (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
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

        Vector3 targetPosition = target.position + cameraOfset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
