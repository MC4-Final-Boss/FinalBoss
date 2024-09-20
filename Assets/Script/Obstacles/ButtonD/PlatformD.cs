using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformD : MonoBehaviour
{
    public GameObject buttonD1Object;
    public GameObject buttonD2Object;

    public GameObject movementPlatform;
    private ButtonD1 buttonD1Status;
    private ButtonD2 buttonD2Status;
    public bool platformMove = false;
    public float platformSpeed = 2;
    public bool buttonActive = false;
    public Vector3 targetPosition;
    private Vector3 initialPosition;

    private void Start()
    {
        buttonD1Status = buttonD1Object.GetComponent<ButtonD1>();
        buttonD2Status = buttonD2Object.GetComponent<ButtonD2>();
        initialPosition = transform.position;

    }

    void MoveTowards(Vector3 target)
    {
        transform.position = Vector3.MoveTowards(transform.position, target, platformSpeed * Time.deltaTime);
    }

    private void Update()
    {



        if (buttonD1Status.buttonD1 && buttonD2Status.buttonD2)
        {
            Debug.Log("Start Move1 : " + buttonD1Status.buttonD1);
            Debug.Log("Start Move2 : " + buttonD2Status.buttonD2);
            buttonActive = true;
            platformMove = true;
        }
        else
        {
            buttonActive = false;
        }

        // if (platformMove)
        // {
            Vector3 targetPos = buttonActive ? targetPosition : initialPosition;

            movementPlatform.transform.position = Vector3.MoveTowards(
                movementPlatform.transform.position,
                targetPos,
                platformSpeed * Time.deltaTime
            );

            if (Vector3.Distance(movementPlatform.transform.position, targetPos) < 0.01f)
            {
                platformMove = false;
            }
        // }
    }

}
