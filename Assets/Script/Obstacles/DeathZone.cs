using UnityEngine;
using Unity.Netcode;

public class DeathZone : MonoBehaviour
{
    private GameStateManager gameStateManager;

    private void Start()
    {
        // Find the GameStateManager in the scene
        gameStateManager = FindObjectOfType<GameStateManager>();
        if (gameStateManager == null)
        {
            Debug.LogError("GameStateManager not found in the scene!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object that entered the trigger is either Tanko or Gaspi
        if (other.CompareTag("Tanko") || other.CompareTag("Gaspi"))
        {
            // Get the PlayerRespawn component of the character that entered the death zone
            PlayerRespawn respawnScript = other.GetComponent<PlayerRespawn>();
            Debug.Log("Respawn script found for: " + other.gameObject.name);
            
            // Call respawn for the player that entered the death zone
            if (respawnScript != null)
            {
                // Set the game state to Die
                gameStateManager.OnPlayerDie();

                // Respawn the player that entered the death zone
                respawnScript.RespawnPlayer();
            }

            // Find the other player and call its respawn method as well
            RespawnOtherPlayer(other.CompareTag("Tanko"));
        }
    }

    private void RespawnOtherPlayer(bool isTanko)
    {
        string otherTag = isTanko ? "Gaspi" : "Tanko";
        GameObject otherPlayer = GameObject.FindGameObjectWithTag(otherTag);

        if (otherPlayer != null)
        {
            PlayerRespawn otherRespawnScript = otherPlayer.GetComponent<PlayerRespawn>();
            if (otherRespawnScript != null)
            {
                // Respawn the other character
                otherRespawnScript.RespawnPlayer(); 
            }
        }
    }
}
