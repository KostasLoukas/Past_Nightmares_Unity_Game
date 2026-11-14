using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCeilingsAndWalls : MonoBehaviour
{
    [SerializeField] private GameObject hideKidsBedroomWall;
    [SerializeField] private GameObject hideBedroomHallWall;
    [SerializeField] private MeshRenderer hallWall;
    
    

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hideKidsBedroomWall.SetActive(true);
            hideBedroomHallWall.SetActive(true);
            hallWall.enabled = true;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hideKidsBedroomWall.SetActive(false);
            hideBedroomHallWall.SetActive(false);
            hallWall.enabled = false;
        }
    }
}
