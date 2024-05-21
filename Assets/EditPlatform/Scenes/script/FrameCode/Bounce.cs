using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 通过解析式模拟物体自由落体并反弹的往复运动
// 可以用来与Explicit Euler方式的模拟进行对比
public class Bounce : MonoBehaviour
{
    private double g = 9.79; // gravity
    private double h0; // initial position 
    private double height; // current position
    private double t; // elapsed time
    private float x;
    private float z;
    private double v0; // initial velocity
    private double v; // current velocity

    // Start is called before the first frame update
    void Start()
    {
        // set initial values
        height = transform.position.y;
        h0 = height;
        x = transform.position.x;
        z = transform.position.z;
        t = 0;
        v0 = 0;
        v = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // update elapsed time
        t = t + Time.deltaTime;
        // calculate new position
        UpdateHeight();
        // set new position
        transform.position = new Vector3(x, (float)height, z);
        
    }

    // Analytical Solution
    void UpdateHeight()
    {
        // 1. calculate displacement
        double delta = v0 * t - g * t * t / 2;
        // 2. calculate current height with initial height and displacement 
        height = h0 + delta;
        // 3. calculate current velocity
        v = v0 - g * t;
        // 4. change direction if needed, reset initial values
        //     case 1: reach the bottom
        if (height <= 0)
        {
            h0 = 0;
            v0 = g * t;
            t = 0;
        }
        //     case 2: reach the peak
        else if (v <= 0 && v0 > 0)
        {
            h0 = height;
            v0 = 0;
            t = 0;
        }
    }
}
