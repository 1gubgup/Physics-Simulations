using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 通过解析式模拟抛物运动
// 可以用来与Explicit Euler方式的模拟进行对比
public class Throw : MonoBehaviour
{
    // initial position 
    private float h0; 
    private float x0;
    private float z0;
    // current position
    private float height; 
    private float x;
    private float z;
    // initial velocity, try different values
    private Vector3 v0 = new Vector3(15, 2, 0);
    // current velocity
    private Vector3 v;
    private float t; // elapsed time
    private float g = 9.79f; // gravity

    // Start is called before the first frame update
    void Start()
    {
        // set initial values
        height = transform.position.y;
        h0 = height;
        x = transform.position.x;
        x0 = x;
        z = transform.position.z;
        z0 = z;
        t = 0;
        v = Vector3.zero;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // update elapsed time
        t = t + Time.deltaTime;
        // calculate new position
        UpdateHeight();
        // set new position
        transform.position = new Vector3(x, height, z);

    }

    // Analytical Solution
    void UpdateHeight()
    {
        // 1. calculate displacement
        float deltay = v0.y * t - g * t * t / 2;
        float deltax = v0.x * t;
        float deltaz = v0.z * t;
        // 2. calculate current position with initial position and displacement 
        height = h0 + deltay;
        x = x0 + deltax;
        z = z0 + deltaz;
        // 3. calculate current velocity, v only changes in y-axis
        v.y = v0.y - g * t;
        // 4. reach the bottom, stay static
        if (height <= 0)
        {
            h0 = 0;
            x0 = x;
            z0 = z;
            v0 = Vector3.zero;
            t = 0;
        }
    }
}