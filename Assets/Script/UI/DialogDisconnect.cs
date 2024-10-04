using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DialogDisconnect : MonoBehaviour
{
    [SerializeField] private GameObject disconnectAlertPanel; // The UI panel for the disconnect alert
    [SerializeField] private TextMeshProUGUI disconnectMessageText; // The text component for displaying the message
    [SerializeField] private Button okButton; // The OK button
    [SerializeField] private GameObject controllerCanvas;

    private void Start()
    {
        // Ensure the panel is disabled at the start
        disconnectAlertPanel.SetActive(false);

        // Attach the OK button listener
        okButton.onClick.AddListener(OnOkButtonClicked);
    }

    // Method to show the disconnect alert
    public void ShowDisconnectAlert(string message)
    {
        Debug.LogWarning(message);
        
        // Set the message in the text component
        disconnectMessageText.text = message;

        // Deactive controller canvas
        controllerCanvas.SetActive(false);

        // Activate the disconnect alert panel
        disconnectAlertPanel.SetActive(true);
    }

    // Called when the OK button is clicked
    private void OnOkButtonClicked()
    {
        // Hide the panel
        disconnectAlertPanel.SetActive(false);

        // Load the menu scene
        SceneManager.LoadScene("RizuMenuScene");
    }

    // This function can be called when a disconnect happens in your game (for example, in a network manager or connection handler)
    private void OnClientDisconnected()
    {
        ShowDisconnectAlert("You have been disconnected from the server.");
    }
}
