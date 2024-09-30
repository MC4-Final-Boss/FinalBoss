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
    private bool sceneLoaded = false; // Variable to prevent scene reloading

    private void Update()
    {
        // Set buttonActive to true only when both Gaspi and Tanko are on the button
        buttonActive = gaspiTriggered && tankoTriggered;

        // Load scene only if the button is activated and scene hasn't been loaded yet
        if (buttonActive && !sceneLoaded)
        {
            // Ensure only the server loads the scene
            if (NetworkManager.Singleton.IsServer)
            {
                LoadGameScene();
            }
        }
    }

    private void LoadGameScene()
    {
        Debug.Log("Loading game scene 'BustlingCityScene'");
        sceneLoaded = true; // Prevents further scene loading

        // Use the NetworkManager's SceneManager to load the scene for all clients
        NetworkManager.Singleton.SceneManager.LoadScene("BustlingCityScene", LoadSceneMode.Single);
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
