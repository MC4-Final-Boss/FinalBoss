using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class DialogDisconnect : MonoBehaviour
{
    [SerializeField] private GameObject disconnectAlertPanel;
    [SerializeField] private GameObject controllerPanel;
    [SerializeField] private TextMeshProUGUI disconnectMessageText;
    [SerializeField] private Button okButton;

    public static DialogDisconnect Instance { get; private set; }

    private void Awake()
    {
        // Singleton pattern to ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        disconnectAlertPanel.SetActive(false);
        okButton.onClick.AddListener(OnOkButtonClicked);
        
        // Subscribe to the scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Find and cache the controller panel in the new scene if it exists
        GameObject newControllerPanel = GameObject.FindGameObjectWithTag("Controller");
        if (newControllerPanel != null)
        {
            controllerPanel = newControllerPanel;
        }
    }

   

    public void ShowDisconnectAlert(string message)
    {
        // Check if the panel still exists
        if (disconnectAlertPanel == null)
        {
            Debug.LogError("Disconnect Alert Panel is missing. Cannot show alert.");
            return;
        }

        if (controllerPanel != null)
        {
            controllerPanel.SetActive(false);
        }

        Debug.LogWarning(message);
        disconnectMessageText.text = message;
        disconnectAlertPanel.SetActive(true);
    }


    private void OnOkButtonClicked()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }
        disconnectAlertPanel.SetActive(false);
        SceneManager.LoadScene("RizuMenuScene");
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}