using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractionPromptUI : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField] private GameObject uiPanel;
    //[SerializeField] public TextMeshProUGUI promptText;
    public bool isDisplayed = false;



    void Start()
    {
        //initialization
        mainCamera = Camera.main;
        uiPanel.SetActive(false);
    }


    void LateUpdate()
    {
        var _rotation = mainCamera.transform.rotation;
        transform.LookAt(transform.position + _rotation * Vector3.forward, _rotation * Vector3.up);
    }


    public void Setup(string prompt)
    {
        //promptText.text = prompt;
        uiPanel.SetActive(true);
        isDisplayed = true;
    }


    public void Close()
    {
        uiPanel.SetActive(false);
        isDisplayed = false;
    }
}
