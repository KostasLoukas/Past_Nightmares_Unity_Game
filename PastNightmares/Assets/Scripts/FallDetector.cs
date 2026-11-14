using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallDetector : MonoBehaviour
{
    //public static FallDetector fallDetector;
    public Transform target;
    private float nextTimeToSearchForPlayer = 0;
    private GameObject player;




    // Update is called once per frame
    void Update()
    {
        if (target == null)  //don't run the code below if there is no player to follow
        {
            FindPlayer();
            return;
        }

        this.transform.position = new Vector3(target.transform.position.x, this.transform.position.y, target.transform.position.z);

    }


    void FindPlayer()
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
    }
}
