using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class IntroUI : MonoBehaviour
{

    [SerializeField] private Button playButton; 


     private void Start() 
    {
        // Ensure button listener is set in the Start or Awake method
        playButton.onClick.AddListener(() => 
        {
            SceneManager.LoadScene("MenuScene"); // Loads the "Game" scene
            Debug.Log("IntroUI: Play button clicked");
        });
    }

    
}
