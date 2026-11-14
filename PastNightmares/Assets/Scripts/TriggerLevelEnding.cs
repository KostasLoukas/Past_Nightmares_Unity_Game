using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class TriggerLevelEnding : MonoBehaviour, IInteractable
{
    [SerializeField] private string prompt;
    public string InteractionPrompt => prompt;  //returns the prompt
    public bool endingTriggered = false;
    public float timeUntilEndingStarts = 0f;
    private bool stopTimer = false;
    [SerializeField] private AudioSource exitHouseDoor;
    [SerializeField] private AudioClip doorBellRing;
    public float doorBellRingTimer = 0f;
    public bool playerInteractedWithExit = false;
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject endingCamera;
    [SerializeField] private Animator fadeToWhiteImage;
    [SerializeField] private AudioClip fadeToWhiteSound;
    private bool endGame = false;
    private PlayerInput playerInput;
    private PauseMenu pauseMenu;
    [SerializeField] private GameObject kitchenVideo;
    [SerializeField] private GameObject kitchenCutsceneVideoPlayer;
    [SerializeField] private TMP_Text playerObjective;
    [SerializeField] private Animator playerObjectiveAnimator;
    [SerializeField] private GameObject endingLevelCanvas;
    [SerializeField] private GameObject endingLevelVideo;
    [SerializeField] private GameObject levelEndingVideoPlayer;
    public string creditsScene;





    void Awake()
    {
        playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
        pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu").GetComponent<PauseMenu>();
        endingCamera.SetActive(false);
        endingLevelVideo.SetActive(false);
        levelEndingVideoPlayer.SetActive(false);
        endingLevelCanvas.SetActive(false);
        kitchenVideo.SetActive(false);
        kitchenCutsceneVideoPlayer.SetActive(false);

    }


    // Update is called once per frame
    void Update()
    {
        if (endingTriggered)
        {
            if (timeUntilEndingStarts > 0 && stopTimer == false)
            {
                timeUntilEndingStarts -= Time.deltaTime;
            }
            else if (timeUntilEndingStarts <= 0 && stopTimer == false)  //start the ending sequence of events
            {
                stopTimer = true;
                InvokeRepeating("DoorBellRing", 0f, doorBellRingTimer);  //run function every 15 seconds
            }
        }

        if (playerInteractedWithExit && !endGame)
        {
            CancelInvoke();
            exitHouseDoor.Stop();
            SoundManager.instance.PauseMusic();

            mainCamera.SetActive(false);
            endingCamera.SetActive(true);
            endingLevelCanvas.SetActive(true);
            playerInput.enabled = false; //Disable player's input so that he cannot move when the ending is playing
            pauseMenu.enabled = false; //Disable the pause menu so that the player cannot pause the game when the ending sequence is playing
            endingCamera.GetComponent<Animator>().SetTrigger("ZoomIn");
            fadeToWhiteImage.SetTrigger("FadeToWhite");
            exitHouseDoor.PlayOneShot(fadeToWhiteSound);

            endGame = true; //So that this is not called more than once

            StartCoroutine(PlayEndingCutscene());
        }
    }


    IEnumerator PlayEndingCutscene()
    {
        yield return new WaitForSeconds(2);

        fadeToWhiteImage.SetTrigger("FadeFromWhite");

        endingCamera.SetActive(false);
        mainCamera.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        levelEndingVideoPlayer.SetActive(true);
        endingLevelVideo.SetActive(true);

        yield return new WaitForSeconds(34);

        SceneManager.LoadSceneAsync(creditsScene);

    }
    
    private void DoorBellRing()
    {
        exitHouseDoor.PlayOneShot(doorBellRing);
    }


    public bool Interact (Interactor interactor)  //returns true if the interaction was successful and false if it was unsuccessful
    {
        if (endingTriggered == false && GameMaster.gm.introHasPlayed)
        {
            StartCoroutine(PlayPreEndingCutscene());
        }

        return true;
    }

    IEnumerator PlayPreEndingCutscene()
    {
        gameObject.layer = 0;  //set this gameObject's layer to default (we don't need it to be interactable more than once)
        endingLevelCanvas.SetActive(true);
        playerInput.enabled = false; //Disable player's input so that he cannot move when the ending is playing
        pauseMenu.enabled = false; //Disable the pause menu so that the player cannot pause the game when the ending sequence is playing
        InspectableObject.playerIsInspecting = true;  //don't let the player crouch while playing the cut-scene

        fadeToWhiteImage.SetTrigger("FadeToWhite");
        yield return new WaitForSeconds(1);
        fadeToWhiteImage.SetTrigger("FadeFromWhite");
        //Cursor.lockState = CursorLockMode.None;
        SoundManager.instance.PauseMusic();  //no music from here on out..?


        //Play the fridge cut-scene here like the one with the drawing
        kitchenCutsceneVideoPlayer.SetActive(true);
        kitchenVideo.SetActive(true);
        yield return new WaitForSeconds(12);
        kitchenCutsceneVideoPlayer.SetActive(false);
        kitchenVideo.SetActive(false);


        //enable the disabled features of the player
        playerInput.enabled = true;
        InspectableObject.playerIsInspecting = false;
        pauseMenu.enabled = true;
        fadeToWhiteImage.SetTrigger("FadeFromWhite");
        yield return new WaitForSeconds(1);
        endingLevelCanvas.SetActive(false);
        //Cursor.lockState = CursorLockMode.Locked;
        //SoundManager.instance.ResumeMusic();

        endingTriggered = true;

        playerObjectiveAnimator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(2);
        playerObjective.text = "Answer the door.";
        yield return new WaitForSeconds(1);
        playerObjectiveAnimator.SetTrigger("FadeIn");

        yield return null;
    }


    /*private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && endingTriggered == false)
        {
            endingTriggered = true;
        }
    }*/

}
