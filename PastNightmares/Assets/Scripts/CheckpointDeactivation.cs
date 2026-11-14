using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointDeactivation : MonoBehaviour
{
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameObject.GetComponent<BoxCollider>().enabled = false;  //deactivate checkpoint's collider so it cannot override the most recent one's coordinates for player's respawn point
        }
    }
}
