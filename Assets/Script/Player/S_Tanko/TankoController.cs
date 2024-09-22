using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using Unity.Netcode;

public class TankoController : NetworkBehaviour
{
    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private float jumpForce = 3f;
    public float horizontalAxis;
    private Vector2 direction;
    private int jumpLeft = 1;
    [SerializeField] int pressedPlayer = 0;
    private float lag;  // Track network lag

    [SerializeField] private float fallThreshold = -15f;

    [SerializeField] private Rigidbody2D rb;
    Animator animator;



    void Start()
    {
        // if (!IsOwner) return;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Cek apakah karakter ini dimiliki oleh pemain lokal
        // if (!photonView.IsMine)
        // {
        //     // Jika karakter ini bukan milik pemain lokal, nonaktifkan skrip kontrol ini
        //     this.enabled = false;
        // }

        //animator = GetComponent<Animator>();

    }

    void Update()
    {
        // Jika karakter bukan milik pemain lokal, jangan jalankan input
        
        if (!IsOwner) return;
        Movement();
        Jump();
        Facing();
        Animations();
    }


    void Movement()
    {
        // Modify the input for horizontal movement to use 'A' and 'D' keys
        if (Input.GetKey(KeyCode.A))  // Move left
        {
            horizontalAxis = -1f;  // Moving left
        }
        else if (Input.GetKey(KeyCode.D))  // Move right
        {
            horizontalAxis = 1f;   // Moving right
        }
        else
        {
            horizontalAxis = 0f;   // Idle
        }

        direction = new Vector2(horizontalAxis, 0);
        transform.Translate(direction * Time.deltaTime * movementSpeed);  // Move character
    }


    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.W) && jumpLeft > 0)
        {
            if (pressedPlayer == 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpLeft--;
            }
        }
    }

    void Animations()
    {
        animator.SetFloat("Moving", Mathf.Abs(horizontalAxis));

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
        // Cek apakah objek yang keluar dari trigger adalah "Tanko"
        if (other.gameObject.CompareTag("Gaspi"))
        {
            // Jika Tanko keluar dari trigger, Gaspi bisa melompat lagi
            jumpLeft = 1;
            pressedPlayer = 0;
            Debug.Log("Tanko bisa melompat lagi, Tanko sudah tidak ada di atasnya!");
        }
    }


    void Facing()
    {
        Vector3 playerScale = transform.localScale;

        if (horizontalAxis < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(playerScale.x), playerScale.y, playerScale.z);
        }
        else if (horizontalAxis > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(playerScale.x), playerScale.y, playerScale.z);
        }
    }






}


