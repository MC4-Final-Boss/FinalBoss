using UnityEngine;
using Photon.Pun;

public class GaspiController : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private float movementSpeed = 7f;  // Gaspi lebih cepat dari Tanko
    [SerializeField] private float jumpForce = 3f;
    public float horizontalAxis;
    [SerializeField] int jumpLeft = 1;
    private Vector2 direction;
    [SerializeField] int pressedPlayer = 0;
    private float lag;  // Track network lag

    [SerializeField] private float fallThreshold = -15f;

    [SerializeField] private Rigidbody2D rb;
    Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Cek apakah karakter ini dimiliki oleh pemain lokal
        if (!photonView.IsMine)
        {
            // Jika karakter ini bukan milik pemain lokal, nonaktifkan skrip kontrol ini
            this.enabled = false;
        }
    }

    void Update()
    {
        // Jika karakter bukan milik pemain lokal, jangan jalankan input
        if (!photonView.IsMine)
        {
            return;
        }

        Movement();
        Jump();
        Facing();
        Animations();
    }

    void Movement()
    {
        // Modify the input for horizontal movement to use 'J' and 'L' keys
        if (Input.GetKey(KeyCode.J))  // Move left
        {
            horizontalAxis = -1f;  // Moving left
        }
        else if (Input.GetKey(KeyCode.L))  // Move right
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
        if (Input.GetKeyDown(KeyCode.I) && jumpLeft > 0)
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
        if (!other.gameObject.CompareTag("Tanko"))
        {
            // Reset jumlah lompatan ketika objek lain masuk
            jumpLeft = 1;
        }

        // Cek apakah objek yang memasuki trigger adalah "Tanko"
        else if (other.gameObject.CompareTag("Tanko"))
        {
            // Cek apakah Tanko berada di atas Gaspi
            if (other.transform.position.y > transform.position.y)
            {
                // Jika Tanko ada di atas Gaspi, cegah lompatan
                Debug.Log("Gaspi tidak bisa melompat, ada Tanko di atasnya!");
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
    if (other.gameObject.CompareTag("Tanko"))
    {
        // Jika Tanko keluar dari trigger, Gaspi bisa melompat lagi
        jumpLeft = 1;
            pressedPlayer = 0;

            Debug.Log("Gaspi bisa melompat lagi, Tanko sudah tidak ada di atasnya!");
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send player's position to the other clients
            stream.SendNext(rb.position);
            stream.SendNext(rb.velocity);
        }
        else
        {
            // Receive the position and velocity from other clients
            direction = (Vector2)stream.ReceiveNext();
            rb.velocity = (Vector2)stream.ReceiveNext();

            // Calculate lag
            lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            direction += rb.velocity * lag;  // Predict the new position based on the lag
        }
    }
}
