using UnityEngine;
using System.Collections;

public class GaspiControllerDummy : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 7f;
    [SerializeField] private float jumpForce = 3f;
    public float horizontalAxis;
    [SerializeField] private int jumpLeft = 1;
    [SerializeField] private int pressedPlayer = 0;
    [SerializeField] private bool explodePlayer = false;
    [SerializeField] private bool drown = false;
    [SerializeField] private float fallThreshold = -15f;

    [SerializeField] private Rigidbody2D rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        Movement();
    }

    void Update()
    {
        HandleInput();
        Facing();
        Animations();
    }

    void HandleInput()
    {
        if (Input.GetKey(KeyCode.J))
            horizontalAxis = -1f;
        else if (Input.GetKey(KeyCode.L))
            horizontalAxis = 1f;
        else
            horizontalAxis = 0f;

        if (Input.GetKeyDown(KeyCode.I) && jumpLeft > 0)
            Jump();
    }

    void Movement()
    {
        Vector2 movement = new Vector2(horizontalAxis * movementSpeed, rb.velocity.y);
        rb.velocity = movement;
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
        animator.SetBool("ExplodePlayer", explodePlayer);
        animator.SetBool("Drown", drown);

    }

    void Facing()
    {
        if (horizontalAxis != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(horizontalAxis) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    // OnTriggerEnter2D untuk menangani kolisi dengan objek lain
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerRespawn respawnScript = GetComponent<PlayerRespawn>();

        if (rb.velocity.y <= fallThreshold)
        {
            if (respawnScript != null)
            {
                StartCoroutine(HandleExplosionAndRespawn(respawnScript));
            }
        }

        if (!other.gameObject.CompareTag("Tanko"))
        {
            if (other.gameObject.CompareTag("Water"))
            {
                StartCoroutine(HandleDrownAndRespawn(respawnScript));
            }
            else if (other.transform.position.y > transform.position.y)
            {
                Debug.Log("Tanko tidak bisa melompat, ada Tanko di atasnya!");
                jumpLeft = 0;
                StartCoroutine(HandleExplosionAndRespawn(respawnScript));

            }
            else
            {
                jumpLeft = 1;
            }
        }

        else if (other.gameObject.CompareTag("Tanko"))
        {
            if (other.transform.position.y > transform.position.y)
            {
                Debug.Log("Gaspi tidak bisa melompat, ada Tanko di atasnya!");
                jumpLeft = 0;
                pressedPlayer = 1;

            }
            else
            {
                jumpLeft = 1;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Tanko"))
        {
            jumpLeft = 1;
            pressedPlayer = 0;
        }
    }

    // Coroutine untuk menangani ledakan dan respawn ketika player jatuh terlalu jauh
    IEnumerator HandleExplosionAndRespawn(PlayerRespawn respawnScript)
    {
        explodePlayer = true; // Aktifkan animasi ledakan

        // Tunggu durasi animasi ledakan
        AnimatorStateInfo animStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animLength = animStateInfo.length;
        yield return new WaitForSeconds(animLength);

        // Reset explodePlayer setelah animasi selesai
        explodePlayer = false;

        // Respawn player setelah animasi selesai
        respawnScript.RespawnPlayer();
        Debug.Log("Player Death and Respawned");
    }

    // Coroutine untuk menangani ledakan dan respawn ketika player jatuh terlalu jauh
    IEnumerator HandleDrownAndRespawn(PlayerRespawn respawnScript)
    {
        drown = true; // Aktifkan animasi ledakan

        // Tunggu durasi animasi ledakan
        AnimatorStateInfo animStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animLength = animStateInfo.length;
        yield return new WaitForSeconds(animLength);

        // Reset explodePlayer setelah animasi selesai
        drown = false;

        // Respawn player setelah animasi selesai
        respawnScript.RespawnPlayer();
        Debug.Log("Player Death and Respawned");
    }

}
