using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float movementSpeed = 7f;
    [SerializeField] private float jumpForce = 3f;
    [SerializeField] private int jumpLeft = 1;
    [SerializeField] private int pressedPlayer = 0;
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
            HandleInput();
            UpdatePositionServerRpc(rb.position);
        }
        else
        {
            // Update local facing based on network variable
            transform.localScale = new Vector3(netFacingRight.Value ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
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

        if (Input.GetKeyDown(KeyCode.I) && jumpLeft > 0)
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
        if (IsOwner && pressedPlayer == 0 && jumpLeft > 0)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpLeft--;

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

    [ServerRpc]
    private void UpdateFacingServerRpc(bool isFacingRight)
    {
        netFacingRight.Value = isFacingRight;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsOwner)
        {
            if (!other.gameObject.CompareTag("Tanko") && !other.gameObject.CompareTag("Gaspi"))
            {
                jumpLeft = 1;
            }
            else if (other.gameObject.CompareTag("Tanko") || other.gameObject.CompareTag("Gaspi"))
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
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if ((other.gameObject.CompareTag("Tanko") || other.gameObject.CompareTag("Gaspi")))
        {
            jumpLeft = 1;
            pressedPlayer = 0;
        }
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
