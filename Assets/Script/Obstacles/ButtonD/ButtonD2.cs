using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonD2 : MonoBehaviour
{
    public GameObject pressButton1;
    public bool buttonD2;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Gaspi") || other.gameObject.CompareTag("Tanko"))
        {
            buttonD2 = true;
            Debug.Log("Button 2 : " + buttonD2);

        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Gaspi") || other.gameObject.CompareTag("Tanko"))
        {
            buttonD2 = false;
            Debug.Log("Button 2 : " + buttonD2);

        }
    }
}
