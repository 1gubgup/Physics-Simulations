using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reset : MonoBehaviour
{
    Vector3 startPosition;
    Quaternion startRotation;
    Vector3 startVelocity;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = this.transform.position;
        startRotation = this.transform.rotation;
        startVelocity = new Vector3(0,0,0);
    }

    public void resetPlayer()
    {
        this.transform.position = startPosition;
        this.transform.rotation = startRotation;
        this.GetComponent<Rigidbody>().velocity = startVelocity;
    }
}
