using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    public GameObject buttonGaspi;
    public GameObject buttonTanko;

    private bool gaspiTriggered = false;
    private bool tankoTriggered = false;

    public bool buttonActive = false;

    private void Update()
    {
        // Load scene only when both conditions are met
        if (gaspiTriggered && tankoTriggered)
        {
            buttonActive = true;
        }
        else
        {
            buttonActive = false;
        }

        LoadGameScene();
    }

    private void LoadGameScene()
    {
        if (buttonActive == true)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("BustlingCityScene", LoadSceneMode.Single);
        }
    }

    // Detect when an object with tag "Gaspi" or "Tanko" enters the button collider
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Gaspi"))
        {
            gaspiTriggered = true;
            Debug.Log("Gaspi entered the collider");
        }
        else if (other.CompareTag("Tanko"))
        {
            tankoTriggered = true;
            Debug.Log("Tanko entered the collider");
        }
    }

    // Detect when an object with tag "Gaspi" or "Tanko" exits the button collider
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Gaspi"))
        {
            gaspiTriggered = false;
            Debug.Log("Gaspi exited the collider");
        }
        else if (other.CompareTag("Tanko"))
        {
            tankoTriggered = false;
            Debug.Log("Tanko exited the collider");
        }
    }
}
