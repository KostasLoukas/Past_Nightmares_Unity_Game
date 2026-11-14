using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.InputSystem;

public class InspectableObject : MonoBehaviour, IInteractable
{
    [SerializeField] private string prompt;
    public string InteractionPrompt => prompt;  //returns the prompt

    protected Vector3 posLastFrame;
    private Vector3 lastPos;
    private Quaternion lastRot;
    [SerializeField] private Camera cam;
    public Vector3 inspectionOffset;
    private bool followPlayer = false;
    private Vector2 turn;
    public float rotationSpeed = 10f;
    [SerializeField] private GameObject inspectionCanvas;
    [SerializeField] private TextMeshProUGUI objectNameTMP;
    [SerializeField] private TextMeshProUGUI objectDescriptionTMP;
    public string objectName;
    public string objectDescription;
    private GameObject player;
    private PlayerInput playerInput;
    private PlayerPickupDrop playerPickupDrop;
    public static bool playerIsInspecting = false;



    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerInput = player.GetComponent<PlayerInput>();
        playerPickupDrop = player.GetComponent<PlayerPickupDrop>();
    }
    


    void Start()
    {
        inspectionCanvas.SetActive(false);
        lastPos = transform.position;
        lastRot = transform.rotation;
    }


    void Update()
    {
        
        if (followPlayer)
        {
            transform.position = cam.transform.position + inspectionOffset; //unnecessary if the player is not moving...

            if (!PauseMenu.GameIsPaused)  //don't rotate the object if tha game is paused
            {
                turn.x = Input.GetAxis("Mouse X") * rotationSpeed;
                turn.y = Input.GetAxis("Mouse Y") * rotationSpeed;

                Vector3 right = Vector3.Cross(cam.transform.up, transform.position - cam.transform.position);
                Vector2 up = Vector3.Cross(transform.position - cam.transform.position, right);
                transform.rotation = Quaternion.AngleAxis(-turn.x, up) * transform.rotation;
                transform.rotation = Quaternion.AngleAxis(turn.y, right) * transform.rotation;

            }

            if (Input.GetKeyDown(KeyCode.Q) && !PauseMenu.GameIsPaused)
            {
                transform.position = lastPos;
                transform.rotation = lastRot;
                followPlayer = false;
                playerIsInspecting = false;
                inspectionCanvas.SetActive(false);
                playerInput.enabled = true;
                playerPickupDrop.enabled = true;
            }
        }

        if(GameMaster.gm.playerIsDead) //do all of the above but don't enable the PlayerInput
        {
            transform.position = lastPos;
            transform.rotation = lastRot;
            followPlayer = false;
            inspectionCanvas.SetActive(false);
            //playerInput.enabled = true;
            playerPickupDrop.enabled = true;
        }
    }


    public bool Interact (Interactor interactor)  //returns true if the interaction was successful and false if it was unsuccessful
    {

        if (GameMaster.gm.playerIsDead == false && !playerIsInspecting)  //don't interact with InspectableObject if the player is dead or is already inspecting another
        {
            lastPos = transform.position;
            lastRot = transform.rotation;
            
            followPlayer = true;
            playerIsInspecting = true;

            inspectionCanvas.SetActive(true);

            playerInput.enabled = false;
            playerPickupDrop.enabled = false;

            objectNameTMP.text = objectName;
            objectDescriptionTMP.text = objectDescription;
            
        }

        return true;
    }

    
}
