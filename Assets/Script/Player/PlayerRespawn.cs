using UnityEngine;
using Unity.Netcode;

public class PlayerRespawn : NetworkBehaviour
{
    private static Vector3 initialTankoPosition = new Vector3(-5, 0, 0);
    private static Vector3 initialGaspiPosition = new Vector3(-4, 0, 0);
    private PlayerSaveCheckPoint saveCheckpoint;

    private void Start()
    {
        saveCheckpoint = GetComponent<PlayerSaveCheckPoint>();
        if (saveCheckpoint == null)
        {
            Debug.LogError("PlayerSaveCheckPoint component is missing! " + gameObject.name);
        }
    }

    // This method is now responsible for determining the respawn position
    private Vector3 GetRespawnPosition()
    {
        // Return the initial position based on whether the player is Tanko or Gaspi
        return IsHost ? initialTankoPosition : initialGaspiPosition;
    }

    public void RespawnPlayer()
    {
        Vector3 respawnPosition = GetRespawnPosition(); // Get the respawn position
        RespawnPlayerClientRpc(respawnPosition); // Call the networked respawn
    }

    [ClientRpc]
    private void RespawnPlayerClientRpc(Vector3 position)
    {
        // Only execute this on the owner of the object
        if (!IsOwner) return;

        transform.position = position; // Respawn the player at the given position
        Debug.Log($"Respawned {gameObject.name} at position: {position}");
    }
}
