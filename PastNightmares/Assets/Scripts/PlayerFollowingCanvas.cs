using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollowingCanvas : MonoBehaviour
{
    private Camera mainCamera;
    

    // Start is called before the first frame update
    void Start()
    {
        //initialization
        mainCamera = Camera.main;
    }

    
    void LateUpdate()
    {
        var _rotation = mainCamera.transform.rotation;
        transform.LookAt(transform.position + _rotation * Vector3.forward, _rotation * Vector3.up);
    }
}
