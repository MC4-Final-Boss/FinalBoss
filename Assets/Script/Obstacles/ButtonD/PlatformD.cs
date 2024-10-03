using System.Collections;
using UnityEngine;

public class PlatformD : MonoBehaviour
{
    public GameObject buttonD1Object;
    public GameObject buttonD2Object;
    public GameObject movementPlatform;

    private ButtonD1 buttonD1Status;
    private ButtonD2 buttonD2Status;
    public bool platformMove = false;
    public float platformSpeed = 2f;
    public bool buttonActive = false;
    public Vector3 targetPosition;
    private Vector3 initialPosition;

     private SFXManager sfxManager; 

    private bool isWaiting = false; // To track if the platform is already waiting

    private void Start()
    {
        buttonD1Status = buttonD1Object.GetComponent<ButtonD1>();
        buttonD2Status = buttonD2Object.GetComponent<ButtonD2>();
        initialPosition = movementPlatform.transform.localPosition; // Get the initial position of the platform
        sfxManager = FindObjectOfType<SFXManager>();
    }

    private void Update()
    {
        // Check if both buttons are pressed
        if (buttonD1Status.buttonD1 && buttonD2Status.buttonD2)
        {
            StopAllCoroutines(); // Stop any active coroutine when the platform should move to the target
            isWaiting = false; // Reset the waiting flag
            buttonActive = true;
            platformMove = true;
        }
        else if (buttonActive) // When buttons are released
        {
            // Start coroutine to wait for 3 seconds before moving back
            if (!isWaiting)
            {
                StartCoroutine(WaitBeforeReturn());
            }
        }

        // Determine target position based on button state
        Vector3 targetPos = buttonActive ? targetPosition : initialPosition;

        // Move the platform towards the target position
        movementPlatform.transform.localPosition = Vector3.MoveTowards(
            movementPlatform.transform.localPosition,
            targetPos,
            platformSpeed * Time.deltaTime
        );

        // Stop movement when the platform reaches the target position
        if (Vector3.Distance(movementPlatform.transform.localPosition, targetPos) < 0.01f)
        {
            platformMove = false;
        }
    }

    // Coroutine to wait for 3 seconds before returning to the initial position
    private IEnumerator WaitBeforeReturn()
    {
        isWaiting = true; // Set the waiting flag to true
        yield return new WaitForSeconds(3); // Wait for 3 seconds

        buttonActive = false; // Set the buttonActive to false to move the platform back
        isWaiting = false; // Reset the waiting flag
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        other.transform.SetParent(movementPlatform.transform); // Attach the object to the platform
        // Play button press sound
        if (sfxManager != null)
        {
            sfxManager.PlayPlatformMovingSFX();
        }
    }

}
