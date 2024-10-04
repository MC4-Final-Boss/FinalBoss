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
    public float collisionOffset = 0.2f; // Distance to keep from obstacles
    public LayerMask collisionLayers; // Layers to check for collision

    private Vector3 currentCameraOffset;
    private float maxDistance;

    private void Awake()
    {
        StartCoroutine(FindPlayer());
        currentCameraOffset = cameraOffset;
        maxDistance = cameraOffset.magnitude;
    }

    IEnumerator FindPlayer()
    {
        bool isHost = NetworkManager.Singleton.IsHost;
        
        while (target == null)
        {
            GameObject player = null;
            if (isHost)
            {
                player = GameObject.FindGameObjectWithTag("Tanko");
            }
            else
            {
                player = GameObject.FindGameObjectWithTag("Gaspi");
            }
            
            if (player != null)
            {
                target = player.transform;
            }
            yield return null;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + cameraOffset;
        Vector3 adjustedPosition = HandleCameraCollision(target.position, desiredPosition);
        
        transform.position = Vector3.SmoothDamp(transform.position, adjustedPosition, ref velocity, smoothTime);
    }

    private Vector3 HandleCameraCollision(Vector3 fromPosition, Vector3 toPosition)
    {
        RaycastHit hit;
        Vector3 direction = (toPosition - fromPosition).normalized;
        float distance = Vector3.Distance(fromPosition, toPosition);

        if (Physics.Raycast(fromPosition, direction, out hit, distance, collisionLayers))
        {
            // If there's an obstacle, position the camera at the hit point plus a small offset
            return hit.point - (direction * collisionOffset);
        }

        return toPosition;
    }

    // Optional: Visualize the camera ray in the editor
    private void OnDrawGizmos()
    {
        if (target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(target.position, target.position + cameraOffset);
        }
    }
}