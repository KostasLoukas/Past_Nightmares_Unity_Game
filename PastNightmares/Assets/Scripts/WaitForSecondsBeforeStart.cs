using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using UnityEngine.InputSystem;
using TMPro;

public class WaitForSecondsBeforeStart : MonoBehaviour
{
    private PauseMenu pauseMenu;
    GameObject introductionCanvas;
    [SerializeField] private AudioClip introClip;
    [SerializeField] private AudioClip playerWakeUpGasp;
    [SerializeField] private AudioClip levelNameIntroClip;
    [SerializeField] private AudioClip level1IntroClip;
    private Animator playerAnimator;
    private PlayerInput playerInput;
    private PlayerPickupDrop playerPickupDrop;
    private AudioSource audioSource;
    [SerializeField] private TMP_Text playerObjective;
    [SerializeField] private Animator playerObjectiveAnimator;
    


    
    void Awake()
    {
        pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu").GetComponent<PauseMenu>();
        introductionCanvas = GameObject.FindGameObjectWithTag("IntroductionCanvas");
        playerAnimator = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
        playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
        playerPickupDrop = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPickupDrop>();

    }

    
    void Start()
    {
        //Disable the player input while the introduction is playing:
        playerInput.enabled = false;  //for moving around
        playerPickupDrop.enabled = false;  //for picking up/dropping items
        InspectableObject.playerIsInspecting = true;  //for crouching

        //Disable the Pause Menu until the game starts
        pauseMenu.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        
        StartCoroutine(WaitForStartCoroutine());
        
    }

    IEnumerator WaitForStartCoroutine()
    {
        //Start the introductory sound effects clip and wait till the intro is finished
        SoundManager.instance.PlaySFXSound(introClip);
        yield return new WaitForSeconds(37);
        //Print the time the intro ended
        Debug.Log("Finished WaitForStart Coroutine at timestamp : " + Time.time);

        //Enable the Pause Menu after the game starts
        pauseMenu.GetComponent<PauseMenu>().enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        
        //'Wake up' the player and wait till he's standing
        playerAnimator.SetTrigger("WakeUp");
        SoundManager.instance.PlaySFXSound(playerWakeUpGasp);
        yield return new WaitForSeconds(7);

        //Enable player input after the introduction is finished:
        playerInput.enabled = true;  //for moving around
        InspectableObject.playerIsInspecting = false;  //for crouching


        //Wait to play the level's name animation and sound
        SoundManager.instance.PlaySFXSound(levelNameIntroClip);
        yield return new WaitForSeconds(10);
        playerPickupDrop.enabled = true;  //enable this a little later as it seems to be cutting the intro text short...


        //Close the introduction canvas
        introductionCanvas.GetComponent<Canvas>().enabled = false;

        //Start the first level's background music (on loop)
        if (GameMaster.gm.ambientMusicTriggered == false) //don't play the level's music if the player rushed into the attic..
        {
            SoundManager.instance.PlayMusicSound(level1IntroClip);
        }
        playerObjective.text = "Make something to eat in the kitchen.";
        playerObjectiveAnimator.SetTrigger("FadeIn");
        
        GameMaster.gm.introHasPlayed = true;
        
    }
    
}
