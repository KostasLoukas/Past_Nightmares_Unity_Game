using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CustomRigidBodyPush : MonoBehaviour
{
    [SerializeField] private float forceMagnitude;
    private Animator animator;
    public bool isPushing = false;



    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }


    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rigidbody = hit.collider.attachedRigidbody;

        //The && here is to seclude the two push methods available (6 = the number of the PushableObject layer)
        if (rigidbody != null && rigidbody.gameObject.layer != 6)
        {
            //Vector3 forceDirection = hit.gameObject.transform.position - transform.position;
            //forceDirection.y = 0;
            Vector3 forceDirection = new Vector3(hit.moveDirection.x, 0.0f, hit.moveDirection.z);
            forceDirection.Normalize();
            
            
            rigidbody.AddForceAtPosition(forceDirection*forceMagnitude, transform.position, ForceMode.Impulse);
            
        }
        
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.collider.tag == "HeavyObject")
        {
            animator.SetBool("Push", true);
            isPushing = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.collider.tag == "HeavyObject")
        {
            animator.SetBool("Push", false);
            isPushing = false;
        }
    }
    
}
