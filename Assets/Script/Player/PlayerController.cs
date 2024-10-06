using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Linq;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float movementSpeed = 7f;
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private bool OnGround = true;
    [SerializeField] private bool isCollidingWithObjectBelow = true;
    [SerializeField] private int pressedPlayer = 0;
    [SerializeField] private bool explodePlayer = false;
    [SerializeField] private bool drown = false;
    [SerializeField] private float fallThreshold = -15f;
    [SerializeField] private bool isFalling = false;
    private SFXManager sfxManager;
    

    [SerializeField] private float jumpVelocityThreshold = 0.1f;
    private PlayerRespawn respawnScript;

    private Button leftButton;
    private Button rightButton;
    private Button jumpButton;
    private Button restartButton;
    [SerializeField] private Rigidbody2D rb;
    private Animator animator;
    private Vector3 movement;

    private NetworkVariable<float> netVelocityX = new NetworkVariable<float>();
    private NetworkVariable<float> netVelocityY = new NetworkVariable<float>();
    private NetworkVariable<Vector2> netPosition = new NetworkVariable<Vector2>();
    private NetworkVariable<bool> netFacingRight = new NetworkVariable<bool>(true);
    private NetworkVariable<float> netAnimationMoving = new NetworkVariable<float>();
    private NetworkVariable<bool> netExplodePlayer = new NetworkVariable<bool>();
    private NetworkVariable<bool> netDrown = new NetworkVariable<bool>();
    private NetworkVariable<bool> netIsFalling = new NetworkVariable<bool>(false);

    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sfxManager = FindObjectOfType<SFXManager>();
        leftButton = GameObject.Find("Left Button").GetComponent<Button>();
        rightButton = GameObject.Find("Right Button").GetComponent<Button>();
        jumpButton = GameObject.Find("Jump Button").GetComponent<Button>();
        restartButton = GameObject.Find("Restart Button").GetComponent<Button>();
        respawnScript = GetComponent<PlayerRespawn>();
        respawnScript.RespawnPlayer();
        Debug.Log("Player start to respawn");



        if (IsOwner)
        {
            SetupButtons();
        }

    
    }

    void SetupButtons()
    {
        AddButtonEvent(leftButton, () => movement = Vector3.left, () => movement = Vector3.zero);
        AddButtonEvent(rightButton, () => movement = Vector3.right, () => movement = Vector3.zero);
        AddButtonEvent(jumpButton, Jump, null);
        restartButton.onClick.AddListener(RequestRestartServerRpc);
    }
    
    void FixedUpdate()
    {
        if (IsOwner)
        {
            Movement();

            if (rb.velocity.y <= -15f)
            {
                isFalling = true;
            }
            else
            {
                isFalling = false;
            }

            UpdateIsFallingServerRpc(isFalling);

            UpdateVelocityServerRpc(rb.velocity.x, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(netVelocityX.Value, netVelocityY.Value);
            rb.MovePosition(netPosition.Value);
            isFalling = netIsFalling.Value;
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


    [ServerRpc(RequireOwnership = false)]
    private void RequestRestartServerRpc()
    {
        RestartClientRpc();
    }

    [ClientRpc]
    private void RestartClientRpc()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("NewBustlingCityScene", LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene("NewBustlingCityScene");
        }
    }


    void Movement()
    {
        float currentYVelocity = rb.velocity.y;

        if (currentYVelocity > 6f)
        {
            currentYVelocity = 6f;
        }


        Vector2 currentMovement = new Vector2(movement.x * movementSpeed, currentYVelocity);
        rb.velocity = currentMovement;


        UpdateVelocityServerRpc(currentMovement.x, currentMovement.y);
        //Debug.Log($"x: {rb.velocity.x}, y: {rb.velocity.y}");




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
            Vector2 jumpDirection = Vector2.up;

            if (sfxManager != null)
            {
                sfxManager.PlayJumpingSFX();
            }

            if (movement.x != 0)
            {
                float horizontalJumpForce = jumpForce * 0.5f;
                jumpDirection = new Vector2(movement.x, 1f).normalized;

                rb.AddForce(new Vector2(jumpDirection.x * horizontalJumpForce, jumpDirection.y * jumpForce),
                    ForceMode2D.Impulse);
            }
            else
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }

            if (rb.velocity.y > 6f)
            {
                rb.velocity = new Vector2(rb.velocity.x, 6f);
            }

        }

    }


    void Animations()
    {
        bool isJumping = Mathf.Abs(rb.velocity.y) > jumpVelocityThreshold;
        animator.SetFloat("Moving", isJumping ? 0 : Mathf.Abs(movement.x));
        // animator.SetBool("IsJumping", isJumping);
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

    private int groundCollisionCount = 0;  

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsOwner)
        {
            if (isFalling)
            {
                StartCoroutine(HandleExplodeAndRespawn());
            }

            if (other.gameObject.CompareTag("Water"))
            {
                StartCoroutine(HandleDrownAndRespawn());
                Debug.Log("Player menyentuh air");
            }

            if (other.gameObject.CompareTag("Tanko") || other.gameObject.CompareTag("Gaspi"))
            {
                if (other.transform.position.y > transform.position.y)
                {
                    OnGround = false;
                }
                else
                {
                    isCollidingWithObjectBelow = true;
                    OnGround = true;
                    groundCollisionCount++;
                }
            }
            else
            {
                if (other.transform.position.y > transform.position.y && other.gameObject.CompareTag("BasicBox"))
                {
                    if (respawnScript != null)
                    {
                        StartCoroutine(HandleExplodeAndRespawn());
                        Debug.Log("Ada sesuatu di atasnya");
                    }
                    return;
                }
                else
                {
                    if (other.gameObject.CompareTag("Ground"))
                    {
                        rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, 0));
                        isCollidingWithObjectBelow = true;
                        OnGround = true;
                        groundCollisionCount++;
                    }
                    else
                    {
                        isCollidingWithObjectBelow = true;
                        OnGround = true;
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Tanko") || other.gameObject.CompareTag("Gaspi"))
        {
            if (other.transform.position.y > transform.position.y)
            {
                if (isCollidingWithObjectBelow)
                {
                    OnGround = true;
                }
                else
                {
                    OnGround = false;
                }
            }
            else
            {
                isCollidingWithObjectBelow = false;
                groundCollisionCount--; 
                if (groundCollisionCount <= 0)
                {
                    OnGround = false;
                }
            }
        }
        else if (other.gameObject.CompareTag("Ground"))
        {
            isCollidingWithObjectBelow = false;
            groundCollisionCount--; 
            if (groundCollisionCount <= 0)  // Jika tidak ada lagi objek yang disentuh
            {
                OnGround = false;
            }
        }
    }


    IEnumerator HandleExplodeAndRespawn()
    {
        if (sfxManager != null)
        {
            sfxManager.PlayExplodingSFX();
        }
        explodePlayer = true;

        AnimatorStateInfo animStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animLength = animStateInfo.length;
        yield return new WaitForSeconds(animLength);

        // respawnScript.RespawnPlayer();
        RequestRestartServerRpc();
        Debug.Log("Player Death and Respawned");

        explodePlayer = false;
    }


    IEnumerator HandleDrownAndRespawn()
    {
        drown = true;

        yield return new WaitForSeconds(2f);

        //respawnScript.RespawnPlayer();
        RequestRestartServerRpc();
        Debug.Log("Player tenggelam");

        drown = false;
    }


    [ServerRpc]
    private void UpdateIsFallingServerRpc(bool newIsFalling)
    {
        netIsFalling.Value = newIsFalling;
    }


    [ServerRpc]
    private void UpdateVelocityServerRpc(float newVelocityX, float newVelocityY)
    {
        netVelocityX.Value = newVelocityX;
        netVelocityY.Value = newVelocityY;
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

    [ServerRpc(RequireOwnership = false)]
    public void SetParentServerRpc(string playerName, string platformName)
    {
        Debug.Log("PARENT IS CALLED");
        GameObject platform = GameObject.Find(platformName);
        transform.SetParent(platform.transform);
    }

    [ServerRpc(RequireOwnership = false)]
    public void UnsetParentServerRpc(string playerName)
    {
        transform.SetParent(null);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Platform"))
        {
            UnsetParentServerRpc(gameObject.name);
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Platform"))
        {
            SetParentServerRpc(gameObject.name, other.gameObject.name);
        }
    }

}
