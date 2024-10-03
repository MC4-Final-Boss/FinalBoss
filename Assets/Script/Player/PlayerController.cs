using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Linq;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float movementSpeed = 7f;
    [SerializeField] private float jumpForce = 3f;
    [SerializeField] private bool OnGround = true;
    [SerializeField] private int pressedPlayer = 0;
    [SerializeField] private bool explodePlayer = false;
    [SerializeField] private bool drown = false;
    [SerializeField] private float fallThreshold = -15f;
    private SFXManager sfxManager;

    [SerializeField] private float jumpVelocityThreshold = 0.1f;

    private Button leftButton;
    private Button rightButton;
    private Button jumpButton;

    [SerializeField] private Rigidbody2D rb;
    private Animator animator;
    private Vector3 movement;

    private NetworkVariable<Vector2> netPosition = new NetworkVariable<Vector2>();
    private NetworkVariable<bool> netFacingRight = new NetworkVariable<bool>(true);
    private NetworkVariable<float> netAnimationMoving = new NetworkVariable<float>();
    private NetworkVariable<bool> netExplodePlayer = new NetworkVariable<bool>();
    private NetworkVariable<bool> netDrown = new NetworkVariable<bool>();

    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sfxManager = FindObjectOfType<SFXManager>();
        leftButton = GameObject.Find("Left Button").GetComponent<Button>();
        rightButton = GameObject.Find("Right Button").GetComponent<Button>();
        jumpButton = GameObject.Find("Jump Button").GetComponent<Button>();

        if (IsOwner)
        {
            SetupButtons();
        }
    }

    void SetupButtons()
    {
        AddButtonEvent(leftButton, () => movement = Vector3.left, () => movement = Vector3.zero);
        AddButtonEvent(rightButton, () => movement = Vector3.right, () => movement = Vector3.zero);

        if (jumpButton != null)
        {
            jumpButton.onClick.AddListener(Jump);
        }
    }

    void FixedUpdate()
    {
        if (IsOwner)
        {
            Movement();
        }
        else
        {
            rb.MovePosition(netPosition.Value);
        }
    }

    void Update()
    {
        if (IsOwner)
        {
            Facing();
            Animations();
            UpdatePositionServerRpc(rb.position);
            UpdateAnimationMovingServerRpc(Mathf.Abs(movement.x));
            UpdateAnimationBoolsServerRpc(explodePlayer, drown);
        }
        else
        {
            // Update local facing based on network variable
            transform.localScale = new Vector3(netFacingRight.Value ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            animator.SetFloat("Moving", netAnimationMoving.Value);
            animator.SetBool("ExplodePlayer", netExplodePlayer.Value);
            animator.SetBool("Drown", netDrown.Value);
        }
    }


    void Movement()
    {
        float currentYVelocity = rb.velocity.y;
        Vector2 currentMovement = new Vector2(movement.x * movementSpeed, currentYVelocity);

        rb.velocity = currentMovement;

        if (Mathf.Abs(movement.x) > 0.01f)
        {
            if (sfxManager != null)
            {
                sfxManager.PlayWalkingSFX();
            }
        }
        else
        {
            if (sfxManager != null)
            {
                sfxManager.StopWalkingSFX();
            }
        }
        
    }


    public void Jump()
    {
        if (IsOwner && pressedPlayer == 0 && OnGround)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            // Play jumping sound
            if (sfxManager != null)
            {
                sfxManager.PlayJumpingSFX();
            }
        }
    }

    void Animations()
    {
        bool isJumping = Mathf.Abs(rb.velocity.y) > jumpVelocityThreshold;
        animator.SetFloat("Moving", isJumping ? 0 : Mathf.Abs(movement.x));
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("ExplodePlayer", explodePlayer);
        animator.SetBool("Drown", drown);
    }

    void Facing()
    {
        if (movement.x != 0)
        {
            bool isFacingRight = movement.x > 0;
            transform.localScale = new Vector3(isFacingRight ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            UpdateFacingServerRpc(isFacingRight);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsOwner)
        {
            PlayerRespawn respawnScript = GetComponent<PlayerRespawn>();

            // Memeriksa kecepatan jatuh
            if (rb.velocity.y <= fallThreshold)
            {
                if (respawnScript != null)
                {
                    // Jalankan Coroutine untuk ledakan dan respawn
                    StartCoroutine(HandleExplosionAndRespawn(respawnScript));
                }
                return;
            }

            if (other.gameObject.CompareTag("Water"))
            {
                // Debug: Pastikan OnTriggerEnter mendeteksi air
                Debug.Log("Player touched water, starting HandleDrownAndRespawn...");
                StartCoroutine(HandleDrownAndRespawn(respawnScript));
            }

            if (other.gameObject.CompareTag("Tanko") || other.gameObject.CompareTag("Gaspi"))
            {
                if (other.transform.position.y > transform.position.y)
                {
                    OnGround = false;
                    drown = true;
                    Debug.Log("Ada player diatasnya");
                }
                else
                {
                    OnGround = true;
                }
            }
            else // Jika objek lain bukan Gaspi atau Tanko
            {
                // Jika berada di atas objek ini
                if (other.transform.position.y > transform.position.y && other.gameObject.CompareTag("BasicBox"))
                {
                    if (respawnScript != null)
                    {
                        // Jalankan Coroutine untuk ledakan dan respawn
                        StartCoroutine(HandleExplosionAndRespawn(respawnScript));
                        Debug.Log("Ada sesuatu diatasnya");
                    }
                    return;
                }
                else
                {
                    if (other.gameObject.CompareTag("Ground"))
                    {
                        rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, 0));
                        OnGround = true;
                    }
                    else
                    {
                        OnGround = true; // Objek lain berada di bawah
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Cek apakah objek yang keluar adalah "Tanko" atau "Gaspi"
        if (other.gameObject.CompareTag("Tanko") || other.gameObject.CompareTag("Gaspi"))
        {
            // Cek apakah posisi y dari "Tanko" atau "Gaspi" lebih besar dari posisi y objek saat ini
            if (other.transform.position.y > transform.position.y)
            {
                // Cek apakah objek saat ini sedang berkolisi dengan objek lain
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f);
                bool isCollidingWithOther = false;

                foreach (Collider2D collider in colliders)
                {
                    // Abaikan objek ini sendiri
                    if (collider.gameObject != gameObject && collider.transform.position.y < transform.position.y)
                    {
                        isCollidingWithOther = true;
                        break;
                    }
                }

                // Jika ada objek lain di bawah objek saat ini, set OnGround ke true
                if (isCollidingWithOther)
                {
                    OnGround = true;
                }
                // Jika tidak ada objek di bawah, set OnGround ke false dan drown ke false
                else
                {
                    OnGround = false;
                    drown = false;
                }
            }
            else
            {
                // Jika posisi y "Tanko" atau "Gaspi" lebih kecil dari posisi y objek saat ini
                OnGround = false;
            }
        }
        else
        {
            // Jika bukan "Tanko" atau "Gaspi", set OnGround ke false
            OnGround = false;
        }
    }


    // Coroutine untuk menangani ledakan dan respawn ketika player jatuh terlalu jauh
    IEnumerator HandleExplosionAndRespawn(PlayerRespawn respawnScript)
    {
        explodePlayer = true; // Aktifkan animasi ledakan

        // Play explosion sound effect
        if (sfxManager != null)
        {
            sfxManager.PlayExplodingSFX();
        }

        // Tunggu durasi animasi ledakan
        AnimatorStateInfo animStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animLength = animStateInfo.length;
        float customDuration = animLength * 2f; // Ganti durasi jika perlu
        yield return new WaitForSeconds(customDuration); // Tunggu sesuai durasi kustom

        // Reset explodePlayer setelah animasi selesai
        explodePlayer = false;

        // Respawn player setelah animasi selesai
        respawnScript.RespawnPlayer();
        Debug.Log("Player Death and Respawned");
    }


    IEnumerator HandleDrownAndRespawn(PlayerRespawn respawnScript)
    {
        drown = true; // Aktifkan animasi tenggelam

        yield return new WaitForSeconds(2f); // Tunggu 2 detik

        // Respawn player setelah durasi
        respawnScript.RespawnPlayer();
        Debug.Log("Player Death and Respawned");

        // Reset drown
        drown = false;

    }


    [ServerRpc]
    private void UpdateAnimationMovingServerRpc(float newMovingValue)
    {
        netAnimationMoving.Value = newMovingValue;
    }

    [ServerRpc]
    private void UpdateAnimationBoolsServerRpc(bool newExplodePlayer, bool newDrown)
    {
        netExplodePlayer.Value = newExplodePlayer;
        netDrown.Value = newDrown;
    }

    [ServerRpc]
    private void UpdateFacingServerRpc(bool isFacingRight)
    {
        netFacingRight.Value = isFacingRight;
    }

    [ServerRpc]
    private void UpdatePositionServerRpc(Vector2 newPosition)
    {
        netPosition.Value = newPosition;
    }

    private void AddButtonEvent(Button button, Action onPress, Action onRelease)
    {
        if (button == null) return;

        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry pointerDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        pointerDown.callback.AddListener((data) => { onPress?.Invoke(); });

        EventTrigger.Entry pointerUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        pointerUp.callback.AddListener((data) => { onRelease?.Invoke(); });

        trigger.triggers.Add(pointerDown);
        trigger.triggers.Add(pointerUp);
    }

    [ClientRpc]
    public void SetParentClientRpc(string platformName)
    {
        Debug.Log("PARENT IS CALLED");
        GameObject platform = GameObject.Find(platformName);
        transform.SetParent(platform.transform);
    }

    [ClientRpc]
    public void UnsetParentClientRpc()
    {
        transform.SetParent(null);
    }
}
