using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SearchForPlayerForFollowCamera : MonoBehaviour
{
    private Transform target;
    private CinemachineVirtualCamera cinemachineVirtualCamera;

    private float nextTimeToSearchForPlayer = 0;


    // Start is called before the first frame update
    void Start()
    {
        cinemachineVirtualCamera = GameObject.FindGameObjectWithTag("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            FindPlayer();
            return;
        }
    }


    void FindPlayer()
    {
        if (nextTimeToSearchForPlayer <= Time.time) //if the point in time we want to search has passed or is now...
        {
            GameObject searchResult = GameObject.FindGameObjectWithTag("CinemachineTarget");

            if (searchResult != null)
            {
                target = searchResult.transform;
                //target.position = new Vector3(target.position.x, target.position.y, 0f);
                cinemachineVirtualCamera.Follow = target;
            }

            nextTimeToSearchForPlayer = Time.time + 0.5f;
        }
    }
}
