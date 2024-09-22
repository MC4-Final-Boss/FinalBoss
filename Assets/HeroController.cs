using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float movementSpeed = 5f; // Kecepatan gerak
    [SerializeField] private float jumpForce = 5f; // Daya lompatan
    [SerializeField] private LayerMask groundLayer; // Layer tanah untuk mendeteksi tanah
    [SerializeField] private Transform groundCheck; // Titik untuk mengecek apakah karakter berada di tanah

    private Rigidbody2D rb;
    private bool isGrounded;

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
        // Mengecek jika karakter berada di tanah dan pengguna menekan tombol lompat (misalnya spasi)
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); // Memberikan daya lompat pada karakter
        }
    }
}
