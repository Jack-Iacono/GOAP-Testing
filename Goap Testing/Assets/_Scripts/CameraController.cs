using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform targetObject;


    private void Start()
    {
        transform.rotation = Quaternion.Euler(new Vector3(90,0,0));
    }
    void Update()
    {
        transform.position = new Vector3(targetObject.position.x, 20, targetObject.position.z);
    }
}
