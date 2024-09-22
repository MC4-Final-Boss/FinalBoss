using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float movementSpeed = 5f; // Kecepatan gerak
    [SerializeField] private float jumpForce = 5f; // Daya lompatan

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if(!IsOwner) return;
        Move();
        Jump();
    }

    private void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal"); // Mengambil input horizontal
        Vector2 moveDirection = new Vector2(horizontalInput, 0);
        rb.velocity = new Vector2(moveDirection.x * movementSpeed, rb.velocity.y); // Menggerakkan karakter
    }

    private void Jump()
    {
        // Langsung berikan gaya lompat ketika tombol spasi ditekan
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); // Memberikan daya lompat pada karakter
        }
    }
}