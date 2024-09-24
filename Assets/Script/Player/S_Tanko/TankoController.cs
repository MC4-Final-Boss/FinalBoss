using UnityEngine;
using Unity.Netcode;

public class TankoController : NetworkBehaviour
{
    [SerializeField] private float movementSpeed = 7f;
    [SerializeField] private float jumpForce = 3f;
    public float horizontalAxis;
    [SerializeField] private int jumpLeft = 1;
    [SerializeField] int pressedPlayer = 0;
    [SerializeField] private float fallThreshold = -15f;
    [SerializeField] private Rigidbody2D rb;
    Animator animator;

    private float lag;  // Track network lag

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;
        Movement();
    }

    void Update()
    {
        if (!IsOwner) return;
        HandleInput();
        Facing();
        Animations();
    }

    void HandleInput()
    {
        if (Input.GetKey(KeyCode.A))
            horizontalAxis = -1f;
        else if (Input.GetKey(KeyCode.D))
            horizontalAxis = 1f;
        else
            horizontalAxis = 0f;

        if (Input.GetKeyDown(KeyCode.W) && jumpLeft > 0)
            Jump();
    }

    void Movement()
    {
        Vector2 movement = new Vector2(horizontalAxis * movementSpeed, rb.velocity.y);
        rb.velocity = movement;
        UpdatePositionOnServerRpc(rb.position);
    }

    void Jump()
    {
        if (pressedPlayer == 0)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpLeft--;
        }
    }

    void Animations()
    {
        animator.SetFloat("Moving", Mathf.Abs(horizontalAxis));
    }

    void Facing()
    {
        if (horizontalAxis != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(horizontalAxis) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        // Cek apakah objek yang memasuki trigger bukan "Tanko"
        if (!other.gameObject.CompareTag("Gaspi"))
        {
            // Reset jumlah lompatan ketika objek lain masuk
            jumpLeft = 1;
        }

        // Cek apakah objek yang memasuki trigger adalah "Tanko"
        else if (other.gameObject.CompareTag("Gaspi"))
        {
            // Cek apakah Tanko berada di atas Gaspi
            if (other.transform.position.y > transform.position.y)
            {
                // Jika Tanko ada di atas Gaspi, cegah lompatan
                Debug.Log("Tanko tidak bisa melompat, ada Tanko di atasnya!");
                jumpLeft = 0;
                pressedPlayer = 1;

            }
            else
            {
                jumpLeft = 1;
            }
        }

        if (rb.velocity.y <= fallThreshold)
        {

            PlayerRespawn respawnScript = GetComponent<PlayerRespawn>();

            if (respawnScript != null)
            {
                respawnScript.RespawnPlayer();
                print("Player Death");
            }

        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Gaspi"))
        {
            jumpLeft = 1;
            pressedPlayer = 0;
        }
    }


    [ServerRpc]
    private void UpdatePositionOnServerRpc(Vector2 newPosition)
    {
        UpdatePositionOnClientsClientRpc(newPosition);
    }

    [ClientRpc]
    private void UpdatePositionOnClientsClientRpc(Vector2 newPosition)
    {
        if (!IsOwner)
        {
            rb.position = newPosition;
        }
    }

}


