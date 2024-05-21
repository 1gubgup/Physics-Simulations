using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO:
// 1. 分别使用Explicit Euler, Midpoint, Trapezoid方法实现单摆模拟
// 2. 创建一个名为fixP的对象，其位置即为单摆运动的固定点
// 3. 设置摆长和初始角度，可以修改对应参数，length和theta
// 4. 尝试不同的时间步长，修改step的值
public class PendulumMidp : MonoBehaviour
{
    public GameObject fixP;
    private float g = 9.79f;
    private Vector3 fixedPos; // position of the fixed point
    public float l = 1; // length of pendulum. you can change it!
    public float theta = -15; // initial angle, range [-90 degree, 90 degree]. you can change it!
    private float omega; // angular velocity
    private int step = 2; // time step, you can change it, too!
    private int count; // used to control the frequency of updates

    // Start is called before the first frame update
    void Start()
    {
        // set the fixed position by gameobjtect
        fixedPos = fixP.transform.position;
        // set the initial position
        theta = theta * Mathf.Deg2Rad;
        SetPosition(theta);
        omega = 0;
        count = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        count++;
        if (count >= step)
        {
            UpdatePosition();
            count = 0;
        }
    }

    // calculate the position according to the pendulum length, angle and fixed point
    // angle: angle of rotation since last update, unit is Rad 
    void SetPosition(float angle)
    {
        Vector3 pos = fixedPos;
        pos.x = fixedPos.x + l * Mathf.Sin(theta);
        pos.y = fixedPos.y - l * Mathf.Cos(theta);
        transform.position = pos;
        transform.Rotate(new Vector3(0, 0, angle * Mathf.Rad2Deg));
    }

    // TODO: complete the function
    void UpdatePosition()
    {
        // 1. save the angle and omega of last time step for later use
        float lastTheta = theta;
        float lastOmega = omega;
        // 2. calculate values after a half time step
        float midTheta = lastTheta + lastOmega * Time.deltaTime * step / 2;
        float midOmega = lastOmega - (g / l) * Mathf.Sin(lastTheta) * Time.deltaTime * step / 2;
        // 3. update theta
        theta = lastTheta + midOmega * Time.deltaTime * step;
        // 4. update omega
        omega = lastOmega - (g / l) * Mathf.Sin(midTheta) * Time.deltaTime * step;
        // 5. move the object to the new postion
        float deltaTheta = theta - lastTheta;
        SetPosition(deltaTheta);
    }
}
