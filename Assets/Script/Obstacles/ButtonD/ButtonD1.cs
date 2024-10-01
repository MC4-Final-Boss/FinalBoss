using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonD1 : MonoBehaviour
{
    public GameObject pressButton1;
    public bool buttonD1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Gaspi") || other.gameObject.CompareTag("Tanko"))
        {
            Debug.Log("Button 1 : " + buttonD1);
            buttonD1 = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Gaspi") || other.gameObject.CompareTag("Tanko"))
        {
            // buttonD1 = false;
            Debug.Log("Button 1 : " + buttonD1);

        }
    }

}
