using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour, IInteractable
{
    [SerializeField] private string prompt;
    public string InteractionPrompt => prompt;  //returns the prompt

    //private DialogueTrigger dialogueTrigger;
    public Dialogue dialogue;
    



    public bool Interact (Interactor interactor)  //returns true if the interaction was successful and false if it was unsuccessful
    {
        TriggerDialog();

        return true;
    }



    public void TriggerDialog()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }
}
