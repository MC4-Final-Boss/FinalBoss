// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class CameraFollow : MonoBehaviour
// {
//     public Transform target; 
//     public Vector3 offset;   
//     public float smoothSpeed = 0.225f; 

//     void LateUpdate()
//     {
//         Vector3 desiredPosition = target.position + offset;
//         Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
//         transform.position = smoothedPosition;
//     }
// }

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
            GameObject player = GameObject.FindGameObjectWithTag("Tanko");
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
