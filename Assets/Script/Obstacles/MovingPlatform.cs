using UnityEngine;
using Unity.Netcode;

public class MovingPlatform : NetworkBehaviour
{
    public Vector3 targetPosition;
    public float speed = 1.0f;
    private Vector3 initialPosition;
    private bool movingToTarget = true;
    private PlayerNetworkManager networkManager;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        initialPosition = transform.localPosition;
        // Find the CustomNetworkManagerWithTag in a more robust way
        networkManager = FindObjectOfType<PlayerNetworkManager>();
        if (networkManager == null)
        {
            Debug.LogError("CustomNetworkManagerWithTag not found in the scene!");
        }
    }

    void Update()
    {
        if (!IsServer) return; // Only server controls the movement

        // Check if networkManager is not null before accessing it
        if (networkManager != null && networkManager.isStarted)
        {
            if (movingToTarget)
            {
                MoveTowards(targetPosition);
                if (Vector3.Distance(transform.localPosition, targetPosition) < 0.1f)
                {
                    movingToTarget = false;
                }
            }
            else
            {
                MoveTowards(initialPosition);
                if (Vector3.Distance(transform.localPosition, initialPosition) < 0.1f)
                {
                    movingToTarget = true;
                }
            }
        }
    }

    void MoveTowards(Vector3 target)
    {
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, target, speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        NetworkObject networkObject = other.gameObject.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            SetParentClientRpc(networkObject);
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        NetworkObject networkObject = other.gameObject.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            UnsetParentClientRpc(networkObject);
        }
    }

    [ClientRpc]
    private void SetParentClientRpc(NetworkObjectReference networkObjectRef)
    {
        if (networkObjectRef.TryGet(out NetworkObject networkObject))
        {
            networkObject.transform.SetParent(transform);
        }
    }

    [ClientRpc]
    private void UnsetParentClientRpc(NetworkObjectReference networkObjectRef)
    {
        if (networkObjectRef.TryGet(out NetworkObject networkObject))
        {
            networkObject.transform.SetParent(null);
        }
    }
}