using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class IntroUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button storyButton;
    [SerializeField] private GameObject buttonSoundObject; // Drag the GameObject with the AudioSource to this field

    private AudioSource buttonSound; // To store the AudioSource of the button sound

    private void Start() 
    {
        // Ensure button listener is set in the Start or Awake method
        if (buttonSoundObject != null)
        {
            buttonSound = buttonSoundObject.GetComponent<AudioSource>(); // Get the AudioSource from the GameObject
        }
        else
        {
            Debug.LogWarning("IntroUI: buttonSoundObject is not assigned.");
        }

        playButton.onClick.AddListener(() => 
        {   
            PlayButtonSound(); // Play the button sound
            SceneManager.LoadScene("RizuMenuScene"); // Loads the "RizuMenuScene" scene
            Debug.Log("IntroUI: Play button clicked");
        });

        storyButton.onClick.AddListener(() =>
        {
            PlayButtonSound(); // Play the button sound
            SceneManager.LoadScene("ShabStoryScene"); // Loads the "ShabStoryScene" scene
            Debug.Log("IntroUI: Story button clicked");
        });

    }

    private void PlayButtonSound()
    {
        if (buttonSound != null)
        {
            buttonSound.Play(); // Play the sound attached to the AudioSource component
        }
        else
        {
            Debug.LogWarning("IntroUI: No AudioSource found on buttonSoundObject.");
        }
    }
}
