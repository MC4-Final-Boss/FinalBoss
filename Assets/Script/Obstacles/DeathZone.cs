using UnityEngine;
using Unity.Netcode;

public class DeathZone : NetworkBehaviour
{
    private GameStateManager gameStateManager;
    // private PlayerRespawn playerRespawn;

    private void Start()
    {
        gameStateManager = FindObjectOfType<GameStateManager>();
        // playerRespawn = FindObjectOfType<PlayerRespawn>();

        if (gameStateManager == null)
        {
            Debug.LogError("GameStateManager not found in the scene!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Tanko") || other.CompareTag("Gaspi"))
        {
            if (IsServer)
            {
                RespawnAllPlayersServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RespawnAllPlayersServerRpc()
    {
        RespawnAllPlayersClientRpc();
    }

    [ClientRpc]
    private void RespawnAllPlayersClientRpc()
    {
        PlayerRespawn[] players = FindObjectsOfType<PlayerRespawn>();
        foreach (PlayerRespawn player in players)
        {
            player.RespawnPlayerClientRpc();
        }
    }
}