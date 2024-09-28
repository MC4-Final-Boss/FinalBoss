using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonLevel : MonoBehaviour
{
    [SerializeField] private GameObject buttonB; // GameObject for button B
    [SerializeField] private GameObject buttonC; // GameObject for button C

    private bool gaspiOnButtonB = false;
    private bool tankoOnButtonC = false;

    void Start()
    {
        // Ensure button GameObjects are assigned in the editor
        if (buttonB == null || buttonC == null)
        {
            Debug.LogError("Button GameObjects are not assigned!");
        }
    }

    void Update()
    {
        // If both conditions are true, change the scene
        if (gaspiOnButtonB && tankoOnButtonC)
        {
            SceneManager.LoadScene("BustlingCityScene");
        }
    }

    // Trigger detection for when "Gaspi" and "Tanko" step on buttonB and buttonC respectively
    private void OnTriggerEnter(Collider other)
    {
        // Check if "Gaspi" steps on buttonB
        if (other.CompareTag("Gaspi") && other.gameObject == buttonB)
        {
            gaspiOnButtonB = true;
        }
        // Check if "Tanko" steps on buttonC
        else if (other.CompareTag("tanko") && other.gameObject == buttonC)
        {
            tankoOnButtonC = true;
        }
    }

    // Detect when "Gaspi" or "Tanko" leaves their respective buttons
    private void OnTriggerExit(Collider other)
    {
        // If "Gaspi" leaves buttonB, set it to false
        if (other.CompareTag("Gaspi") && other.gameObject == buttonB)
        {
            gaspiOnButtonB = false;
        }
        // If "Tanko" leaves buttonC, set it to false
        else if (other.CompareTag("tanko") && other.gameObject == buttonC)
        {
            tankoOnButtonC = false;
        }
    }
}
