using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StoryScene : MonoBehaviour
{

    [SerializeField] private Button nextButton; // next button di story
    [SerializeField] private Button prevButton; // prev button di story
    [SerializeField] private Button mmButton; // main menu button di story
    [SerializeField] GameObject[] background;
    int index;

    // Start is called before the first frame update
    void Start()
    {
        nextButton.gameObject.SetActive(true);
        prevButton.gameObject.SetActive(false);
        mmButton.gameObject.SetActive(false);
        index = 0;

    }

    // Update is called once per frame
    void Update()
    {
        if (index >= 2)
        {
            index = 2;
            nextButton.gameObject.SetActive(false);
            mmButton.gameObject.SetActive(true);
            prevButton.gameObject.SetActive(true);
        }

        if (index < 0)
        {
            index = 0;
            nextButton.gameObject.SetActive(true);
            mmButton.gameObject.SetActive(false);
            prevButton.gameObject.SetActive(false);


        }

        if (index == 1)
        {
            nextButton.gameObject.SetActive(true);
            mmButton.gameObject.SetActive(false);
            prevButton.gameObject.SetActive(true);
        }

        if (index == 0)
        {
            background[0].gameObject.SetActive(true);
            nextButton.gameObject.SetActive(true);
            mmButton.gameObject.SetActive(false);
            prevButton.gameObject.SetActive(false);
        }

    }

    public void Next()
    {
        index += 1;
        for (int i = 0; i < background.Length; i++)
        {
            background[i].gameObject.SetActive(false);
            background[index].gameObject.SetActive(true);
        }
        Debug.Log(index);
    }

    public void Previous()
    {
        index -= 1;
        for (int i = 0; i < background.Length; i++)
        {
            background[i].gameObject.SetActive(false);
            background[index].gameObject.SetActive(true);
        }
        Debug.Log(index);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("RizuIntroScene");
    }

}
