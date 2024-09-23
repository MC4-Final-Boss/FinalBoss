using Unity.Netcode;
using UnityEngine;

public class Player1Observed : NetworkBehaviour
{
    public NetworkVariable<Vector3> playerPosition = new NetworkVariable<Vector3>();

    void Update()
    {
        if (IsOwner)
        {
            // Update player position for sync
            playerPosition.Value = transform.position;
        }
        else
        {
            // Apply synced position
            transform.position = playerPosition.Value;
        }
    }
}
