using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public string InteractionPrompt { get; }
    public bool Interact(Interactor interactor);  //returns true if the interaction was successful and false if it was unsuccessful
    
}
