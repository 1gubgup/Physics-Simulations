using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFly : MonoBehaviour
{
    public GameObject followTarget;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = followTarget.transform.position - followTarget.transform.forward * 30 + followTarget.transform.up * 10;
        transform.LookAt(followTarget.transform.position);
    }
}
