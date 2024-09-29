using UnityEngine;

public class PlayerControllerDummy : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 7f;
    [SerializeField] private float jumpForce = 3f;
    public float horizontalAxis;
    [SerializeField] private int jumpLeft = 1;
    [SerializeField] private int pressedPlayer = 0;
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
        if (!other.gameObject.CompareTag("Tanko"))
        {
            jumpLeft = 1;
        }
        else if (other.gameObject.CompareTag("Tanko"))
        {
            if (transform.position.y < other.transform.position.y)
            {
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
        if (other.gameObject.CompareTag("Tanko"))
        {
            jumpLeft = 1;
            pressedPlayer = 0;
        }
    }

}
