using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class PlayerController : NetworkBehaviour
{
    private SFXManager sfxManager;
    [SerializeField] private float jumpVelocityThreshold = 0.1f;

    private Button leftButton;
    private Button rightButton;
    private Button jumpButton;

    [SerializeField] private float movementSpeed = 7f;
    [SerializeField] private float jumpForce = 3f;
    [SerializeField] private bool isGround = true;

    [SerializeField] private int pressedPlayer = 0;
    [SerializeField] private bool explodePlayer = false;
    [SerializeField] private bool drown = false;
    [SerializeField] private float fallThreshold = -15f;

    [SerializeField] private Rigidbody2D rb;
    private Animator animator;
    private Vector3 movement;

    private NetworkVariable<Vector2> netPosition = new NetworkVariable<Vector2>();
    private NetworkVariable<float> netFacingDirection = new NetworkVariable<float>();
    private NetworkVariable<float> netAnimationMoving = new NetworkVariable<float>();
    private NetworkVariable<bool> netExplodePlayer = new NetworkVariable<bool>();
    private NetworkVariable<bool> netDrown = new NetworkVariable<bool>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sfxManager = FindObjectOfType<SFXManager>();
        GetComponent<Rigidbody2D>().isKinematic = false;
        leftButton = GameObject.Find("Left Button").GetComponent<Button>();
        rightButton = GameObject.Find("Right Button").GetComponent<Button>();
        jumpButton = GameObject.Find("Jump Button").GetComponent<Button>();

        if (IsOwner)
        {
            SetupButtons();
            netFacingDirection.Value = transform.localScale.x;
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
            transform.localScale = new Vector3(netFacingDirection.Value, transform.localScale.y, transform.localScale.z);
        }
    }

    void Update()
    {
        if (IsOwner)
        {
            Facing();
            Animations();
            HandleInput();
            UpdatePositionServerRpc(rb.position);
            UpdateAnimationMovingServerRpc(Mathf.Abs(movement.x));
            UpdateAnimationBoolsServerRpc(explodePlayer, drown);
        }
        else
        {
            animator.SetFloat("Moving", netAnimationMoving.Value);
            animator.SetBool("ExplodePlayer", netExplodePlayer.Value);
            animator.SetBool("Drown", netDrown.Value);
        }
    }

    void HandleInput()
    {
        if (Input.GetKey(KeyCode.J))
            movement.x = -1f;
        else if (Input.GetKey(KeyCode.L))
            movement.x = 1f;
        else
            movement.x = 0f;

        if (Input.GetKeyDown(KeyCode.I) && isGround)
            Jump();
    }

    void Movement()
    {
        Vector2 currentMovement = new Vector2(movement.x * movementSpeed, rb.velocity.y);
        rb.velocity = currentMovement;

        if (Mathf.Abs(movement.x) > 0.01f)
        {
            if (sfxManager != null)
            {
                sfxManager.PlayWalkingSFX(); // Start playing walking sound
            }
        }
        else
        {
            if (sfxManager != null)
            {
                sfxManager.StopWalkingSFX(); // Stop walking sound when not moving
            }
        }
    }

    public void Jump()
    {
        if (IsOwner && pressedPlayer == 0 && isGround)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGround = false;

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
            // Determine the new facing direction based on movement direction
            float newFacingDirection = Mathf.Sign(movement.x) * Mathf.Abs(transform.localScale.x);
            transform.localScale = new Vector3(newFacingDirection, transform.localScale.y, transform.localScale.z);

            // Update facing direction on the server
            UpdateFacingServerRpc(newFacingDirection);
        }
    }

    [ServerRpc]
    private void UpdateFacingServerRpc(float newFacingDirection)
    {
        netFacingDirection.Value = newFacingDirection;
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

            // if (other.gameObject.CompareTag("Water"))
            // {
            //     // Debug: Pastikan OnTriggerEnter mendeteksi air
            //     Debug.Log("Player touched water, starting HandleDrownAndRespawn...");
            //     StartCoroutine(HandleDrownAndRespawn(respawnScript));
            // }

            if (other.gameObject.CompareTag("Tanko") || other.gameObject.CompareTag("Gaspi"))
            {
                if (other.transform.position.y > transform.position.y)
                {
                    isGround = false;
                    drown = true;
                    Debug.Log("Ada player diatasnya");
                }
                else
                {
                    isGround = true;

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
                    isGround = true;

                }
            }
        }
    }


    // Coroutine untuk menangani ledakan dan respawn ketika player jatuh terlalu jauh
    IEnumerator HandleExplosionAndRespawn(PlayerRespawn respawnScript)
    {
        explodePlayer = true; // Aktifkan animasi ledakan

        // Tunggu durasi animasi ledakan
        AnimatorStateInfo animStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animLength = animStateInfo.length;
        float customDuration = animLength * 1f; // Ganti durasi jika perlu
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

        yield return new WaitForSeconds(1f); // Tunggu 2 detik

        // Respawn player setelah durasi
        respawnScript.RespawnPlayer();
        Debug.Log("Player Death and Respawned");

        // Reset drown
        drown = false;

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if ((other.gameObject.CompareTag("Tanko") || other.gameObject.CompareTag("Gaspi")))
        {
            isGround = true;
            pressedPlayer = 0;
            drown = false;

        }
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
}
