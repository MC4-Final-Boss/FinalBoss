using UnityEngine;

public class ButtonBoth : MonoBehaviour
{
    public GameObject pressedButton;
    public GameObject movementGround;
    public Vector3 targetPosition;
    public float moveSpeed = 2f;

    private Vector3 initialPosition;
    private bool platformMove = false;
    private bool buttonActive = false;

    private SFXManager sfxManager; 

    private void Start()
    {
        initialPosition = movementGround.transform.localPosition;  
         sfxManager = FindObjectOfType<SFXManager>();
    }

    private void Update()
    {
        if (platformMove)
        {
            Vector3 targetPos = buttonActive ? targetPosition : initialPosition;

            // Debug.Log("target position: " + targetPosition);
            movementGround.transform.localPosition = Vector3.MoveTowards(
                movementGround.transform.localPosition,  
                targetPos,
                moveSpeed * Time.deltaTime
            );

            if (Vector3.Distance(movementGround.transform.localPosition, targetPos) < 0.01f)
            {
                platformMove = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Tanko") || other.gameObject.CompareTag("Gaspi") || other.gameObject.CompareTag("BasicBox"))
        {
            Debug.Log("Touch Button");
            platformMove = true;
            buttonActive = true;
        }

        // Play button press sound
            if (sfxManager != null)
            {
                sfxManager.PlayButtonPressSFX();
            }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Tanko") || other.gameObject.CompareTag("Gaspi") || other.gameObject.CompareTag("BasicBox"))
        {
            Debug.Log("Tanko left the button");
            platformMove = true;
            buttonActive = false;
        }
    }
}