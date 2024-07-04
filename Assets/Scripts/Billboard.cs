using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    Transform cam;
    void Start()
    {
        cam = Camera.main.transform;
    }

    void Update()
    {
        //transform.LookAt(transform.position + cam.rotation * Vector3.forward, cam.rotation * Vector3.up);
        transform.LookAt(transform.position + cam.rotation * Vector3.forward);
    }
}
