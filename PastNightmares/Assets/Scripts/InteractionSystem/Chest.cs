using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This "chest" can only contain keys for the time being..!
public class Chest : MonoBehaviour, IInteractable
{
    [SerializeField] private string prompt;

    public string InteractionPrompt => prompt;  //returns the prompt

    private bool gotTheKey = false;

    [SerializeField] private AudioClip pickupKeysClip;



    public bool Interact(Interactor interactor)  //returns true if the interaction was successful and false if it was unsuccessful
    {   
        if (gotTheKey == false)
        {
            gotTheKey = true;
            Inventory inventory = interactor.GetComponent<Inventory>();

            inventory.hasKey = true;

            Debug.Log("Interacted with the chest and got a key.");
            SoundManager.instance.PlaySFXSound(pickupKeysClip);
            return true;
        }
        else
        {
            Debug.Log("The chest is now empty.");
            return true;
        }
    }
}
