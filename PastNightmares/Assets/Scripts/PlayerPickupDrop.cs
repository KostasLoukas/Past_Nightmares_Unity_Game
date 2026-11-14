using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerPickupDrop : MonoBehaviour
{
    //[SerializeField] private Transform playerCameraTransform;
    //[SerializeField] private LayerMask pickUpLayerMask;
    [SerializeField] private Transform objectHoldPointTransform;
    [SerializeField] private Transform pickupPoint;
    [SerializeField] private float pickupPointRadius = 0.5f;
    [SerializeField] private LayerMask pickableMask;
    private PickableObject pickableObject;  //this is automatically set by the 'out' command
    private readonly Collider[] colliders = new Collider[3];
    [SerializeField] private int numOfCollidersFound;
    private PickableObject pickable;
    private float holdTimer = 0;
    [SerializeField] private GameObject playerFollowingCanvas;
    public Slider throwForceSlider;
    [SerializeField] private AudioClip chargingThrowClip;
    private AudioSource playerAudioSource;
    private bool clipHasPlayed = false;
    private Animator playerAnimator;
    public bool isCarryingItem = false;
    private ThirdPersonController playerThirdPersonController;



    void Awake()
    {
        playerFollowingCanvas.SetActive(false);
        playerAudioSource = GetComponent<AudioSource>();
        playerAnimator = GetComponent<Animator>();
        playerThirdPersonController = GetComponent<ThirdPersonController>();
    }
    



    void Update()
    {
        if (!playerThirdPersonController.isClimbingLadder) //don't pick up any item if player is climbing a ladder
        {
            if (pickableObject != null)  //don't charge the throw force if an object is NOT being held
            {
                if (Input.GetMouseButton(1) && GameMaster.gm.playerIsDead == false)
                {
                    if(!playerAudioSource.isPlaying && !clipHasPlayed)
                    {
                        SoundManager.instance.PlaySFXSound(chargingThrowClip);
                        clipHasPlayed = true;
                    }

                    playerFollowingCanvas.SetActive(true);

                    if (holdTimer < 25)  //don't exceed this number!
                    {
                        holdTimer += Time.deltaTime*12;
                        throwForceSlider.value = holdTimer;
                        //Debug.Log(holdTimer);
                    }
                    
                }
                else
                {
                    holdTimer = 0;
                    playerFollowingCanvas.SetActive(false);
                    SoundManager.instance.StopSFXSound(chargingThrowClip);
                    clipHasPlayed = false;
                }
            }


            numOfCollidersFound = Physics.OverlapSphereNonAlloc(pickupPoint.position, pickupPointRadius, colliders, pickableMask);

            if (numOfCollidersFound > 0)
            {
                pickable = colliders[0].GetComponent<PickableObject>();

                if (pickable != null)
                {
                    if (Input.GetMouseButtonDown(0) && !playerThirdPersonController.crouch) //don't let the player pick up an item if they're crouching
                    {
                        if (pickableObject == null) //Not carrying an object, try to pick it up
                        {
                            
                            if (pickable.transform.TryGetComponent(out pickableObject)) //check if the object we collide with has the PickableObject script
                            {
                                //Debug.Log("Interacted with pickable object " + pickableObject.gameObject.name);
                                pickableObject.PickUp(objectHoldPointTransform);
                                playerAnimator.SetBool("PickUpItem", true);  //causes bugs
                                isCarryingItem = true;
                            }
                            
                        }
                        else  //Currently carrying something, so drop it
                        {
                            pickableObject.Drop(holdTimer);
                            pickableObject = null;
                            holdTimer = 0;
                            playerFollowingCanvas.SetActive(false);
                            SoundManager.instance.StopSFXSound(chargingThrowClip);
                            playerAnimator.SetBool("PickUpItem", false);
                            isCarryingItem = false;
                        }
                    }
                    if (pickableObject != null && (playerThirdPersonController._input.jump || !playerThirdPersonController.Grounded)) //only drop the item they're holding if the player jumps or is falling
                    {
                        pickableObject.Drop(0);
                        pickableObject = null;
                        holdTimer = 0;
                        playerFollowingCanvas.SetActive(false);
                        SoundManager.instance.StopSFXSound(chargingThrowClip);
                        playerAnimator.SetBool("PickUpItem", false);
                        isCarryingItem = false;
                    }

                }
            }
            
        }
        
        

    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(pickupPoint.position, pickupPointRadius);
    }
    
}
