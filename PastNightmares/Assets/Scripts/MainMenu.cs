using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class MainMenu : MonoBehaviour
{
    public string firstLevel;

    public GameObject optionsScreen;
    public GameObject loadingScreen;
    public GameObject howToPlayScreen;

    public Slider loadingProgressSlider;
    public Text progressText;


    //Start the game
    public void StartGame()
    {
        StartCoroutine(LoadAsynchronously(firstLevel));

        Cursor.lockState = CursorLockMode.Locked;
    }

    IEnumerator LoadAsynchronously (string sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);  //store the status of the loading operation

        loadingScreen.SetActive(true);  //it doesn't need to be set off as the scene will change and it will be discarded anyway

        while (!operation.isDone)  //wait till its done
        {
            float progress = Mathf.Clamp01(operation.progress/0.9f);

            loadingProgressSlider.value = progress;
            progressText.text = progress*100f + "%";

            yield return null;
        }

    }


    //Open the options menu
    public void OpenOptions()
    {
        optionsScreen.SetActive(true);
    }


    //Close the options menu
    public void CloseOptions()
    {
        optionsScreen.SetActive(false);
    }


    public void OpenHowToPlay()
    {
        howToPlayScreen.SetActive(true);
    }


    //Close the options menu
    public void CloseHowToPlay()
    {
        howToPlayScreen.SetActive(false);
    }


    //Quit the game
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quitted.");
    }


}
