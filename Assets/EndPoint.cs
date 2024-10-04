using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class EndPoint : MonoBehaviour
{
    [SerializeField] private GameObject levelClearPanel;
    [SerializeField] private Button retryButton;
    [SerializeField] private string menuSceneName = "RizuMenuScene";
    
    private bool tankoReached;
    private bool gaspiReached;
    private SFXManager sfxManager;

    private void Start()
    {
        sfxManager = FindObjectOfType<SFXManager>();
        levelClearPanel.SetActive(false);
        
        if (retryButton != null)
        {
            retryButton.onClick.AddListener(ReturnToMenu);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // if (!IsServer) return;

        if (other.CompareTag("Tanko"))
            tankoReached = true;
        else if (other.CompareTag("Gaspi"))
            gaspiReached = true;

        if (tankoReached && gaspiReached)
            ShowLevelCompleteClientRpc();
            
        if (sfxManager != null)
            sfxManager.PlayLevelClearSFX();

        levelClearPanel.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // if (!IsServer) return;

        if (other.CompareTag("Tanko"))
            tankoReached = false;
        else if (other.CompareTag("Gaspi"))
            gaspiReached = false;

        levelClearPanel.SetActive(false);
    }

    // [ClientRpc]
    private void ShowLevelCompleteClientRpc()
    {
        levelClearPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    private void ReturnToMenu()
    {
        Time.timeScale = 1f;
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene(menuSceneName);
    }
}