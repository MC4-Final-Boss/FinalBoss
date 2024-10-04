using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class EndPoint : NetworkBehaviour
{
    [SerializeField] private GameObject levelClearPanel;
    [SerializeField] private GameObject controllerPanel;

    [SerializeField] private Button backhomeButton;
    [SerializeField] private Button retryButton;

    [SerializeField] private GameObject bgmManager;
    
    private bool tankoReached;
    private bool gaspiReached;
    private SFXManager sfxManager;


    private void Start()
    {
        sfxManager = FindObjectOfType<SFXManager>();

        levelClearPanel.SetActive(false);
        
        backhomeButton.onClick.AddListener(() => 
        {    
            // NetworkManager.Singleton.Shutdown();
            Time.timeScale = 1f;
            SceneManager.LoadScene("RizuMenuScene"); // Loads the "RizuMenuScene" scene
            Debug.Log("IntroUI: button home clicked");
        });

        retryButton.onClick.AddListener(RequestRestartServerRpc);

         
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        // if (!IsServer) return;
        bgmManager.gameObject.SetActive(false);

        if (other.CompareTag("Tanko"))
            tankoReached = true;
        else if (other.CompareTag("Gaspi"))
            gaspiReached = true;

        if (tankoReached && gaspiReached)
            ShowLevelComplete();
           
            // levelClearPanel.SetActive(true);
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // if (!IsServer) return;

        if (other.CompareTag("Tanko"))
            tankoReached = false;
        else if (other.CompareTag("Gaspi"))
            gaspiReached = false;

        if (tankoReached && gaspiReached)
            levelClearPanel.SetActive(false);
           

        
    }


    // [ClientRpc]
    private void ShowLevelComplete()
    {
        controllerPanel.SetActive(false);
        if (sfxManager != null)
            sfxManager.PlayLevelClearSFX();
        levelClearPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestRestartServerRpc()
    {
        RestartClientRpc();
    }

    [ClientRpc]
    private void RestartClientRpc()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("NewBustlingCityScene", LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene("NewBustlingCityScene");
        }
    }

    
}