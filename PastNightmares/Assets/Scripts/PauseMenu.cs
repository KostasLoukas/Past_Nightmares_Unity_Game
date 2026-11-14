using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using StarterAssets;
using Cinemachine;

public class PauseMenu : MonoBehaviour
{
    
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;
    public GameObject optionsScreen;
    public GameObject howToPlayScreen;
    public GameObject returnToMainMenuConfirmationScreen;
    public GameObject quitGameConfirmationScreen;

    public string mainMenuScene = "MainMenu";

    
    private AudioSource audioSource;
    //private AudioListener playerAudioListener;
    private ThirdPersonController playerThirdPersonController;
    


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)] //resolve gc handle fix
    static void Init()
    {
        GameIsPaused = false;
    }


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.ignoreListenerPause = true;
        //playerAudioListener = GameObject.FindGameObjectWithTag("Player").GetComponent<AudioListener>();
        playerThirdPersonController = GameObject.FindGameObjectWithTag("Player").GetComponent<ThirdPersonController>();
        
    }



    // Update is called once per frame
    void Update()
    {
        /*if (starterAssetsInputs == null)
        {
            FindPlayer();
            return;
        }*/

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                ResumeGame();
                optionsScreen.SetActive(false); //close the options screen if it has been left active
                howToPlayScreen.SetActive(false); //close the how to play screen if it has been left active
                returnToMainMenuConfirmationScreen.SetActive(false); //close the return to main menu confirmation screen if it has been left active
                quitGameConfirmationScreen.SetActive(false); //close the quit game confirmation screen if it has been left active
            }
            else
            {
                PauseGame();
            }
        }
    }


    //Resume the Game
    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;

        /*//Resume the sounds that were playing before the game paused
        AudioSource[] audios = FindObjectsOfType<AudioSource>();
        foreach (AudioSource a in audios)
        {
            a.Play();
        }*/
        AudioListener.pause = false;

        //Lock the cursor again to act as "looking"
        //starterAssetsInputs.cursorInputForLook = true;
        Cursor.lockState = CursorLockMode.Locked;

        /*
        If we use PlayMusicSound function here, the background music of level 1 (or any level) does
        not start again after the game is resumed, probably because it cannot be found in the Music
        channel of the mixer..?
        */
        //SoundManager.instance.StopSFXSound(pauseMenuMusic);
        audioSource.Stop();

        playerThirdPersonController.enabled = true;  //enable the player's movement (and look)
    }


    //Pause the Game
    void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        
        /*//Pause the sounds that were playing
        AudioSource[] audios = FindObjectsOfType<AudioSource>();
        foreach (AudioSource a in audios)
        {
            a.Pause();
        }*/
        AudioListener.pause = true;

        //Unlock the cursor and don't move the camera as if "looking"
        //starterAssetsInputs.cursorInputForLook = false;
        Cursor.lockState = CursorLockMode.None;

        //SoundManager.instance.PlaySFXSound(pauseMenuMusic);
        audioSource.Play();

        playerThirdPersonController.enabled = false;  //disable the player's movement (and look)
    }


    //Go to Main Menu
    public void LoadMenu()
    {
        Debug.Log("Loading Menu...");
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadSceneAsync(mainMenuScene);
    }


    //Open the Options Menu
    public void OpenOptions()
    {
        optionsScreen.SetActive(true);
    }


    //Close the Options Menu
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


    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }

}
