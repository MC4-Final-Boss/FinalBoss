using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BOX : NetworkBehaviour
{
    private Transform playerTransform;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        // Ensure it's a dynamic Rigidbody
        GetComponent<Rigidbody2D>().isKinematic = false;  // Ensure it's a dynamic Rigidbody
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Tanko") || other.gameObject.CompareTag("Gaspi"))
        {
            playerTransform = other.transform; // Store the player's transform

        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Tanko") || other.gameObject.CompareTag("Gaspi"))
        {
            // Clear the reference when the player exits the collision
            playerTransform = null;
        }
    }

    private void Update()
    {
        // If the playerTransform is set, move the box to follow the player's position
        if (playerTransform != null)
        {
            // Move the box towards the player's position
            transform.position = Vector3.Lerp(transform.position, playerTransform.position, Time.deltaTime);
        }
    }
}
