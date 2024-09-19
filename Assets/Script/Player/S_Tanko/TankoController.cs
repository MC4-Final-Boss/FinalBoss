using UnityEngine;

public class TankoController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private float jumpForce = 5f;
    public float horizontalAxis;
    private Vector2 direction;

    [SerializeField] private Rigidbody2D rb;
    // [SerializeField] private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //animator = GetComponent<Animator>();
    }

    void Update()
    {
        Movement();
        Jump();
        // Facing();
    }


    void Movement()
    {
        horizontalAxis = Input.GetAxis("Horizontal");
        direction = new Vector2(horizontalAxis, 0);
        transform.Translate(direction * Time.deltaTime * movementSpeed);

        if (horizontalAxis == 0f)
        {
            // animator.SetBool("Idle", true);
            // animator.SetBool("Run", false);
        }
        else
        {
            // animator.SetBool("Idle", false);
            // animator.SetBool("Run", true);


        }

    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            // animator.SetTrigger("Jump");
        }
    }

    void Facing()
    {
        if (horizontalAxis < 0)
        {
            transform.localScale = new Vector3(-5, 5, 5);
        }
        else if (horizontalAxis > 0)
        {
            transform.localScale = new Vector3(5, 5, 5);
        }
    }
}
