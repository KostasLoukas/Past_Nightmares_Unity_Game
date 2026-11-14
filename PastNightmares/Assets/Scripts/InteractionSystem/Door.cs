using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private string prompt;

    public string InteractionPrompt => prompt;  //returns the prompt

    private Animator doorAnimator;
    private BoxCollider doorCollider;
    [SerializeField] private AudioClip doorLockedClip;
    [SerializeField] private AudioClip doorUnlockedClip;
    [SerializeField] private AudioClip doorOpeningClip;
    [SerializeField] private AudioClip doorClosingClip;
    private AudioSource audioSource;

    private InteractionPromptUI interactionPrompt;
    private bool wasOpened = false;

    [SerializeField] private TriggerLevelEnding triggerLevelEnding;



    void Awake()
    {
        //interactionPrompt = GameObject.FindGameObjectWithTag("InteractionCanvas").GetComponent<InteractionPromptUI>();
        audioSource = GetComponent<AudioSource>();
    }



    public bool Interact (Interactor interactor)  //returns true if the interaction was successful and false if it was unsuccessful
    {
        Inventory inventory = interactor.GetComponent<Inventory>();

        if (inventory == null)
        {
            return false;
        }

        if (gameObject.CompareTag("LockedDoor")) //should unlock it first
        {
            if (wasOpened == false)  //door is locked and closed
            {
                if (inventory.hasKey)
                {
                    Debug.Log("The door unlocked");
                    gameObject.tag = "UnlockedDoor";
                    //interactionPrompt.Setup("The door unlocked"); //this will be visible as long as the player stays near the door
                    doorAnimator = GetComponent<Animator>();
                    doorAnimator.SetTrigger("Open");
                    //SoundManager.instance.PlaySFXSound(doorUnlockedClip);
                    audioSource.PlayOneShot(doorUnlockedClip);
                    //SoundManager.instance.PlaySFXSound(doorOpeningClip.clip);
                    audioSource.PlayOneShot(doorOpeningClip);
                    wasOpened = true;
                    inventory.hasKey = false;
                    return true;
                }
                else
                {

                    if (gameObject.name.Contains("Exit"))  //if this door is the house's exit, act accordingly
                    {
                        if(triggerLevelEnding.endingTriggered)
                        {
                            Debug.Log("Test3");
                            triggerLevelEnding.playerInteractedWithExit = true;
                        }
                        else
                        {
                            audioSource.PlayOneShot(doorLockedClip);
                        }
                        return true;
                    }

                    Debug.Log("The door appears to be locked");
                    //interactionPrompt.Setup("The door appears to be locked"); //this will be visible as long as the player stays near the door
                    //SoundManager.instance.PlaySFXSound(doorLockedClip);
                    audioSource.PlayOneShot(doorLockedClip);
                    return true;
                }
            }
            else
            {
                doorAnimator = GetComponent<Animator>();
                doorAnimator.SetTrigger("Close");
                wasOpened = false;
                return true;
            }
        }
        else if (gameObject.CompareTag("UnlockedDoor") && !audioSource.isPlaying) //don't open/close door if it already opening/closing
        {
            if (wasOpened == false)
            {
                doorAnimator = GetComponent<Animator>();
                doorAnimator.SetTrigger("Open");
                //SoundManager.instance.PlaySFXSound(doorOpeningClip);
                audioSource.PlayOneShot(doorOpeningClip);
                wasOpened = true;
                return true;
            }
            else
            {
                doorAnimator = GetComponent<Animator>();
                doorAnimator.SetTrigger("Close");
                //SoundManager.instance.PlaySFXSound(doorClosingClip);
                audioSource.PlayOneShot(doorClosingClip);
                wasOpened = false;
                return true;
            }
        }
        else
        {
            return false;
        }
        
    }
}
