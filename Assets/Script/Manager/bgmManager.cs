using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMManager : MonoBehaviour
{
    public static BGMManager instance;

    public AudioClip introAndMenuBGM; // BGM for intro and main menu scenes
    public AudioClip gameplayBGM;     // BGM for gameplay scene

    private AudioSource audioSource;

    private string currentScene;

    void Awake()
    {
        // Singleton pattern to make sure there is only one BGMManager in the scene
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.name;
        PlayBGMForCurrentScene();
    }

    private void PlayBGMForCurrentScene()
    {
        AudioClip clipToPlay = null;

        // Check which scene is currently loaded
        if (currentScene == "RizuIntroScene" || currentScene == "RizuMainMenu")
        {
            clipToPlay = introAndMenuBGM;
        }
        else if (currentScene == "NewBustlingCityScene")
        {
            clipToPlay = gameplayBGM;
        }

        if (clipToPlay != null && audioSource.clip != clipToPlay)
        {
            audioSource.clip = clipToPlay;
            audioSource.Play();
        }
    }
}
