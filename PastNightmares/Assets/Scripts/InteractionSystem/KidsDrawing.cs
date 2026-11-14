using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class KidsDrawing : MonoBehaviour, IInteractable
{
    [SerializeField] private string prompt;
    public string InteractionPrompt => prompt;  //returns the prompt

    [SerializeField] private GameObject kidsDrawingBackground;
    [SerializeField] private Animator kidsDrawingBackgroundAnimator;
    [SerializeField] private GameObject kidsDrawingFrame;
    [SerializeField] private Animator kidsDrawingFrameAnimator;
    [SerializeField] private GameObject kidsDrawingVideo;
    [SerializeField] private Animator kidsDrawingVideoAnimator;
    [SerializeField] private GameObject kidsDrawingVideoPlayer;
    private PlayerInput playerInput;
    private PlayerPickupDrop playerPickupDrop;
    private PauseMenu pauseMenu;
    private Interactor interactor;


    

    void Awake()
    {
        //kidsDrawingVideo.SetActive(false);
        kidsDrawingVideoPlayer.SetActive(false);
        playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
        playerPickupDrop = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPickupDrop>();
        pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu").GetComponent<PauseMenu>();
        interactor = GameObject.FindGameObjectWithTag("Player").GetComponent<Interactor>();
        
    }


    public bool Interact (Interactor interactor)  //returns true if the interaction was successful and false if it was unsuccessful
    {
        if (GameMaster.gm.introHasPlayed)
        {
            StartCoroutine(PlayKidsDrawingVideo());

            return true;
        }
        else
        {
            return false;
        }
    }


    IEnumerator PlayKidsDrawingVideo()
    {

        playerInput.enabled = false;  //so that the player cannot move the character while the video is playing
        InspectableObject.playerIsInspecting = true;  //so that the player cannot crouch during this time
        playerPickupDrop.enabled = false; //so that the player cannot pickup or drop items while watching the video
        interactor.enabled = false; //so that the player cannot spam-interact with the drawing
        pauseMenu.enabled = false;  //the player cannot pause the game while watching the video (causes bugs)
        SoundManager.instance.PauseMusic();

        kidsDrawingBackgroundAnimator.SetTrigger("FadeIn");
        kidsDrawingFrameAnimator.SetTrigger("FadeIn");
        kidsDrawingVideoAnimator.SetTrigger("FadeIn");
        kidsDrawingVideo.SetActive(true);
        kidsDrawingVideoPlayer.SetActive(true);
        
        yield return new WaitForSeconds(7);

        kidsDrawingBackgroundAnimator.SetTrigger("FadeOut");
        kidsDrawingFrameAnimator.SetTrigger("FadeOut");
        kidsDrawingVideoAnimator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(1);
        //kidsDrawingVideo.SetActive(false);
        kidsDrawingVideoPlayer.SetActive(false);

        //enable all the player's functions that had been disabled
        playerInput.enabled = true;
        InspectableObject.playerIsInspecting = false;
        playerPickupDrop.enabled = true;
        interactor.enabled = true;
        pauseMenu.enabled = true;
        SoundManager.instance.ResumeMusic();

        yield return null;
    }
    
}
