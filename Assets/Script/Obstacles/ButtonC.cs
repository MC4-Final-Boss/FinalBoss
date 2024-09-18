using UnityEngine;

public class ButtonC
 : MonoBehaviour
{
    public GameObject pressedButton;
    public GameObject movementGround;
    public Vector3 targetPosition;
    public float moveSpeed = 2f;

    private Vector3 initialPosition;
    private bool platformMove = false;
    private bool buttonActive = false;
    private bool isAtTargetPosition = false;

    private void Start()
    {
        initialPosition = movementGround.transform.position;
    }

    private void Update()
    {
        if (platformMove)
        {
            Vector3 targetPos = buttonActive ? targetPosition : initialPosition;

            movementGround.transform.position = Vector3.MoveTowards(
                movementGround.transform.position,
                targetPos,
                moveSpeed * Time.deltaTime
            );

            if (Vector3.Distance(movementGround.transform.position, targetPos) < 0.01f)
            {
                platformMove = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Gaspi") || other.gameObject.CompareTag("Tanko"))
        {
            Debug.Log("Touch Button");
            platformMove = true;
            buttonActive = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Tanko"))
        {
            Debug.Log("Tanko left the button");
            platformMove = true;
            buttonActive = false;
        }
    }
}