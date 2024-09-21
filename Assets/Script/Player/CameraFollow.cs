// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class CameraFollow : MonoBehaviour
// {
//     public Transform target; 
//     public Vector3 offset;   
//     public float smoothSpeed = 0.225f; 

//     void Start()
//     {
//         // Assign target to the first character found, could be Gaspi or Tanko
//         if (target == null)
//         {
//             GameObject player = GameObject.FindWithTag("Tanko");
//             if (player != null)
//             {
//                 target = player.transform; // Assign the player's transform
//             }
//         }
//     }

//     void LateUpdate()
//     {
//         if (target == null) return; // Avoid null reference error
//         Vector3 desiredPosition = target.position + offset;
//         Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
//         transform.position = smoothedPosition;
//     }
// }
