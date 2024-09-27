using UnityEngine;
using Unity.Netcode;

public class MovableBox : NetworkBehaviour
{
    public float moveSpeed = 2.0f;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Allow both host and client to move the box if they interact with it
        if (!IsOwner && !IsClient) return; 

        // Get input for movement (adjust input as needed)
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        // Move box locally for smoother experience and then sync position
        rb.MovePosition(transform.position + movement * moveSpeed * Time.deltaTime);

        // Send the updated position to the server to synchronize with all clients
        MoveBoxServerRpc(transform.position);
    }

    [ServerRpc(RequireOwnership = false)] // Allow all clients to send move requests
    void MoveBoxServerRpc(Vector3 newPosition)
    {
        // Update box position for all clients
        MoveBoxClientRpc(newPosition);
    }

    [ClientRpc]
    void MoveBoxClientRpc(Vector3 newPosition)
    {
        // Move box to the new synchronized position
        rb.MovePosition(newPosition);
    }
}
