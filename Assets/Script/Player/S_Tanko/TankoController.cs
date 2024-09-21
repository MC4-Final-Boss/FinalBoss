using Unity.VisualScripting;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Experimental.GlobalIllumination;

public class TankoController : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private float jumpForce = 3f;
    public float horizontalAxis;
    private Vector2 direction;
    private int jumpLeft = 1;
    private int jumpLeft = 1;
    private float lag;  // Track network lag



    [SerializeField] private Rigidbody2D rb;
    // [SerializeField] private Animator animator;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

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
        // if (!photonView.IsMine)
        // {
        //     return;
        // }

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
        if (Input.GetKeyDown(KeyCode.W) && jumpLeft > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpLeft--;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        jumpLeft = 1;
        Debug.Log("Jump left: " + jumpLeft);
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


