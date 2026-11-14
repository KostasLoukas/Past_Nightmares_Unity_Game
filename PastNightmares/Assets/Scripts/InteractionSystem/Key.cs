using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour, IInteractable
{
    [SerializeField] private string prompt;

    public string InteractionPrompt => prompt;  //returns the prompt

    private bool gotTheKey = false;

    [SerializeField] private AudioClip pickupKeysClip;
    [SerializeField] private AudioClip keysDroppedClip;
    private AudioSource audioSource;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


    public bool Interact(Interactor interactor)  //returns true if the interaction was successful and false if it was unsuccessful
    {   
        if (gotTheKey == false)
        {
            gotTheKey = true;
            Inventory inventory = interactor.GetComponent<Inventory>();

            inventory.hasKey = true;

            Debug.Log("Interacted with the key and put it in the inventory.");
            SoundManager.instance.PlaySFXSound(pickupKeysClip);
            //audioSource.PlayOneShot(pickupKeysClip); //the object is destroyed so the sound never plays :/
            Destroy(gameObject);  //destroy the key
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag == "Ground")
        {
            //SoundManager.instance.PlaySFXSound(keysDroppedClip);
            audioSource.PlayOneShot(keysDroppedClip);
        }
    }
}
