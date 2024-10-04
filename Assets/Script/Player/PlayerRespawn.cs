using UnityEngine;
using Unity.Netcode;

public class PlayerRespawn : NetworkBehaviour
{
    private static Vector3 initialTankoPosition = new Vector3(-6, -2, 0);
    private static Vector3 initialGaspiPosition = new Vector3(-4, -2, 0);
    public PlayerSaveCheckPoint saveCheckpoint;
    private Vector3 respawnPosition;

    private void Start()
    {
        // saveCheckpoint = GetComponent<PlayerSaveCheckPoint>();
        if (saveCheckpoint == null)
        {
            Debug.LogError("PlayerSaveCheckPoint component is missing! " + gameObject.name);
        }
    }

    private Vector3 GetInitialPosition()
    {
        return IsHost ? initialTankoPosition : initialGaspiPosition;
    }

    [ServerRpc(RequireOwnership = false)]
    public void RespawnPlayerServerRpc() 
    {
        RespawnPlayerClientRpc();
    }

    [ClientRpc]
    public void RespawnPlayerClientRpc()
    {
        float checkpointX = PlayerPrefs.GetFloat("CheckpointX");
        Debug.Log("Nilai CheckpointX: " + checkpointX);
        if (PlayerPrefs.GetFloat("CheckpointX") != 0)
        {
            Vector3 checkpointPosition = saveCheckpoint.GetCheckpointPosition();
            
            if (IsHost)
            {
                respawnPosition = checkpointPosition;
                Debug.Log("Host respawning to checkpoint: " + respawnPosition);
            }
            else
            {
                respawnPosition = new Vector3(checkpointPosition.x + 2f, checkpointPosition.y, checkpointPosition.z);
                Debug.Log("Client respawning to offset checkpoint: " + respawnPosition);
            }
        }
        else if (PlayerPrefs.GetFloat("CheckpointX") == 0 || !PlayerPrefs.HasKey("CheckpointX"))
        {
            respawnPosition = GetInitialPosition();
            Debug.Log("Respawning to initial position: " + respawnPosition + " for " + gameObject.name);
        }

        Debug.Log("Player respawn checkpoint status " + !PlayerPrefs.HasKey("CheckpointX"));
        Debug.Log("Player respawn checkpoint value " + PlayerPrefs.GetFloat("CheckpointX"));
        transform.position = respawnPosition; // Update the player position
        Debug.Log($"Respawned {gameObject.name} at position: {respawnPosition}");
    }
}