using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class TankoController : NetworkBehaviour
{
    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private float jumpForce = 5f;
    public float horizontalAxis;
    private Vector2 direction;
    private int jumpLeft = 1;
    private float lag;  // Track network lag


    [SerializeField] private Rigidbody2D rb;
    // [SerializeField] private Animator animator;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

       

        //animator = GetComponent<Animator>();

    }

    void Update()
    {
        // if (!IsOwner) {
        //     return;
        // }
       

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
        if (Input.GetKeyDown(KeyCode.Space) && jumpLeft > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            // animator.SetTrigger("Jump");
            jumpLeft = jumpLeft - 1;
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            jumpLeft = 1;
        }
    }

    

}

