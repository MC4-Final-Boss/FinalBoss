using UnityEngine;
using Photon.Pun;

public class TankoController : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private float jumpForce = 5f;
    public float horizontalAxis;
    private Vector2 direction;

    private float lag;  // Track network lag

    [SerializeField] private Rigidbody2D rb;
    // [SerializeField] private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Cek apakah karakter ini dimiliki oleh pemain lokal
        if (!photonView.IsMine)
        {
            // Jika karakter ini bukan milik pemain lokal, nonaktifkan skrip kontrol ini
            this.enabled = false;
        }

        //animator = GetComponent<Animator>();

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
