using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : MonoBehaviour, IInteractable
{
    [SerializeField] private string prompt;

    public string InteractionPrompt => prompt;  //returns the prompt

    [SerializeField] private GameObject lamp;
    [SerializeField] private AudioClip flipSwitch;
    private bool isOn;
    private AudioSource audioSource;



    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    

    public bool Interact (Interactor interactor)  //returns true if the interaction was successful and false if it was unsuccessful
    {
        audioSource.PlayOneShot(flipSwitch);
        Debug.Log("Interacted with light switch.");

        if (isOn == false)
        {
            lamp.SetActive(true);
            isOn = true;
        }
        else
        {
            lamp.SetActive(false);
            isOn = false;
        }


        return true;
    }
}
