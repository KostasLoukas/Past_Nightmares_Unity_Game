using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableObject : MonoBehaviour
{
    private Rigidbody objectRigidBody;
    //private float forceMagnitude = 10f;
    private Transform objectHoldPointTransform;
    [SerializeField] private AudioClip itemDropClip;
    private AudioSource objectAudioSource;
    private bool hasWaitedForGameStart, hasWaitedForGameStart2 = false;




    void Awake()
    {
        objectRigidBody = GetComponent<Rigidbody>();
        objectAudioSource = GetComponent<AudioSource>();
        objectAudioSource.mute = true;
        hasWaitedForGameStart = false;
        hasWaitedForGameStart2 = false;
    }

    
    void Update()
    {
        if (GameMaster.gm.playerIsDead)  //if player died while holding the object, then make it drop to the ground
        {
            this.objectHoldPointTransform = null;
            objectRigidBody.useGravity = true;
        }
    }


    public void PickUp(Transform _objectHoldPointTransform)
    {
        this.objectHoldPointTransform = _objectHoldPointTransform;
        objectRigidBody.useGravity = false;
        
    }


    public void Drop(float throwForce)
    {
        objectRigidBody.AddRelativeForce(new Vector3(0f, 0.75f, 0.81f) * throwForce, ForceMode.Impulse);

        this.objectHoldPointTransform = null;
        objectRigidBody.useGravity = true;
        
    }


    private void FixedUpdate()
    {
        if (objectHoldPointTransform != null)
        {
            //float lerpSpeed = 10f;

            //Vector3 newPos = Vector3.Lerp(transform.position, objectHoldPointTransform.position, Time.deltaTime*lerpSpeed);
            objectRigidBody.MovePosition(objectHoldPointTransform.position);
            

            //Vector3 newRot = Vector3.Lerp(transform.rotation, objectHoldPointTransform.position, Time.deltaTime*lerpSpeed);
            objectRigidBody.MoveRotation(objectHoldPointTransform.rotation);
            
        }
    }


    private void OnCollisionEnter(Collision other)
    {
        if (!hasWaitedForGameStart && !hasWaitedForGameStart2)  //we don't want to hear the sound on the object's first or second collision! (placement of objects)
        {
            hasWaitedForGameStart = true;
        }
        else if (hasWaitedForGameStart && !hasWaitedForGameStart2)
        {
            hasWaitedForGameStart2 = true;
            objectAudioSource.mute = false;
        }
        else if (hasWaitedForGameStart && hasWaitedForGameStart2)
        {
            if (!other.gameObject.CompareTag("Player")) //don't play the sound of the collider is the player
            {
                objectAudioSource.PlayOneShot(itemDropClip);
            }
        }
    }

    /*private void OnCollisionStay(Collision other)
    {
        if (!hasWaitedForGameStart)
        {
            objectAudioSource.Stop();
        }
    }*/
}
