using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{

    private Transform MainCamera;

    void Start()
    {
        MainCamera = GameObject.FindWithTag("MainCamera").transform;
    }

    // late update is called everyframe after Update()
    void LateUpdate()
    {
        if(MainCamera == null || transform == null) { return; }
        transform.LookAt(
            // if camera rotates this needs to rotate too
            transform.position + MainCamera.rotation * Vector3.forward,
            MainCamera.rotation * Vector3.up);
    }
}
