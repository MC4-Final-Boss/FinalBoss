using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonD2 : MonoBehaviour
{
    public GameObject pressButton1;
    public bool buttonD2;

    private SFXManager sfxManager; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Gaspi"))
        {
            buttonD2 = true;
            Debug.Log("Button 2 : " + buttonD2);

        }

         // Play button press sound
            if (sfxManager != null)
            {
                sfxManager.PlayButtonPressSFX();
            }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Gaspi"))
        {
            buttonD2 = false;
            Debug.Log("Button 2 : " + buttonD2);

        }
    }
}
