using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: 
// 1. 参考注释补全代码，用Explicit Euler方法模拟抛物运动
// 2. 设置不同的初速度，观察运动效果
// 2. 尝试不同大小的时间步长，通过修改step的值来控制步长
// 3. 不同时间步长之间进行对比，以及与解析式方式对比
public class ThrowEuler : MonoBehaviour
{
    private float g = 9.79f;
    private float height;
    private float x;
    private float z;
    private Vector3 v = new Vector3(15,2,0); // Initial velocity, try different valuse

    private int step = 1; // You can change this!
    private int count; // used to control the frequency of updates

    // TODO: complete the function with Explicit Euler method
    void UpdateHeight()
    {
        // 1. update position, move at speed of v for one time step 
        height = height + v.y * Time.deltaTime * step;
        x = x + v.x * Time.deltaTime * step;
        z = z + v.z * Time.deltaTime * step;
        // 2. calculate v in the next time step
        v.y = v.y - g * Time.deltaTime * step;
        // 3. reach the bottom
        if (height <= 0)
        {
            v = Vector3.zero;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // set initial values
        height = transform.position.y;
        x = transform.position.x;
        z = transform.position.z;
        count = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        count++;
        if (count >= step)
        {
            // calculate new position
            UpdateHeight();
            // set new position
            transform.position = new Vector3(x, height, z);
            count = 0;
        }
    }
}
