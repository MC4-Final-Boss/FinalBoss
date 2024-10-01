using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonD1 : MonoBehaviour
{
    public GameObject pressButton1;
    public bool buttonD1;

    // Setelah ini antara function button dan player controller harus dipisah
    private PlayerController actionToggle;


    private void Start()
    {
        // actionToggle = GetComponent<PlayerController>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Gaspi") || other.gameObject.CompareTag("Tanko"))
        {
            // if(actionToggle.toggleOn){
            //     buttonD1 = true;
            //     actionToggle.toggleOn = false;
            // } else if (!actionToggle.toggleOn){
            //     buttonD1 = false;
            //     actionToggle.toggleOn = false;
            // }
            // buttonD1 = true;
            Debug.Log("Button 1 : " + buttonD1);

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
