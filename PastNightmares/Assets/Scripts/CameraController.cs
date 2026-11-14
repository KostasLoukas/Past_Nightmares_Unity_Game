using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public bool useOffsetValues;
    public float yPosRestriction = -1f;

    //private float nextTimeToSearchForPlayer = 0.5f;



    // Start is called before the first frame update
    void Start()
    {
        if (!useOffsetValues)  //manual offset
        {
            offset = target.position - transform.position;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        /*if (target == null)  //don't run the code below if there is no player to follow
        {
            FindPlayer();
            return;
        }*/

        Vector3 newPos = target.position - offset;

        //clamping the camera on the y axis
        newPos = new Vector3(newPos.x, Mathf.Clamp(newPos.y, yPosRestriction, Mathf.Infinity), newPos.z);

        transform.position = newPos;
        
        transform.LookAt(target);
    }


    /*void FindPlayer()
    {
        if (nextTimeToSearchForPlayer <= Time.time) //if the point in time we want to search has passed or is now...
        {
            GameObject searchResult = GameObject.FindGameObjectWithTag("Player");

            if (searchResult != null)
            {
                target = searchResult.transform;
            }

            nextTimeToSearchForPlayer = Time.time + 0.5f;
        }
    }*/

}
