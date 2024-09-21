using UnityEngine;

public class GaspiController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 7f;  // Gaspi lebih cepat dari Tanko
    [SerializeField] private float jumpForce = 7f;
    public float horizontalAxis;
    private Vector2 direction;

    private float lag;  // Track network lag

    [SerializeField] private Rigidbody2D rb;
    //[SerializeField] private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Cek apakah karakter ini dimiliki oleh pemain lokal
       
    }

    void Update()
    {
        // Jika karakter bukan milik pemain lokal, jangan jalankan input
    

        Movement();
        Jump();
        Facing();
    }

    void Movement()
    {
        horizontalAxis = Input.GetAxis("Horizontal");
        direction = new Vector2(horizontalAxis, 0);
        transform.Translate(direction * Time.deltaTime * movementSpeed);

        if (horizontalAxis == 0f)
        {
            //animator.SetTrigger("Idle");
            //animator.SetBool("Idle", true);
            //animator.SetBool("Run", false);
        }
        else
        {
            //animator.SetTrigger("Walk");
            //animator.SetBool("Idle", false);
            //animator.SetBool("Run", true);
        }
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))  // Gunakan tombol panah atas untuk lompat
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            //animator.SetTrigger("Jump");
        }
    }

    void Facing()
    {
        if (horizontalAxis < 0)
        {
            transform.localScale = new Vector3((float)-0.3, (float)0.3, (float)0.3);
        }
        else if (horizontalAxis > 0)
        {
            transform.localScale = new Vector3((float)0.3, (float)0.3, (float)0.3);
        }
    }


}
