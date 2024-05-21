using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PendulumCode
{
    public static string beforeClassName = "// 实验C：模拟单摆运动\n" +
        "// TODO:\n" +
        "// 1. 分别使用Explicit Euler, Midpoint, Trapezoid方法实现单摆模拟\n" +
        "// 2. 创建一个名为fixP的对象，scale设为(1, 1, 1),其位置即为单摆运动的固定点\n" +
        "// 3. 创建一个名为line的立方体对象，scale设为(1, length, 1)，将被作为单摆的摆线\n" +
        "// 4. 设置摆长和初始角度，可以修改对应参数，length和theta\n" +
        "// 5. 尝试不同的时间步长，修改step的值\n" +
        "class ";
    public static string afterClassName = " : MonoBehaviour\n" +
        "{\n" +
        "    private float g = 9.79f;\n" +
        "    private Vector3 fixedPos; // position of the fixed point\n" +
        "    public float length = 50; // length of pendulum. you can change it!\n" +
        "    public float theta = -45; // initial angle, range [-90 degree, 90 degree]. you can change it!\n" +
        "    private float omega; // angular velocity\n" +
        "    private int step = 2; // time step, you can change it, too!\n" +
        "    private int count; // used to control the frequency of updates\n\n" +
        "    // TODO: complete the function\n" +
        "    void UpdatePosition()\n" +
        "    {\n" +
        "        // the difference in angles during this time step\n" +
        "        float deltaTheta = 0;\n" +
        "        // 1. save the theta and omega of last time step for later use\n\n" +
        "        float lastTheta = theta;\n" +
        "        float lastOmega = omega;\n" +
        "        // 2. calculate tmp theta and omega if needed\n\n" +
        "        float midTheta = lastTheta + lastOmega * Time.deltaTime * step / 2;\n" +
        "        float midOmega = lastOmega - (g / length) * Mathf.Sin(lastTheta) * Time.deltaTime * step / 2;\n" +
        "        float newTheta = lastTheta + lastOmega * Time.deltaTime * step;" +
        "        float newOmega = lastOmega - (g / length) * Mathf.Sin(lastTheta) * Time.deltaTime * step;" +
        "        // 3. update theta\n\n" +
        "        deltaTheta= lastOmega * Time.deltaTime * step;\n" +
        "        theta = lastTheta + deltaTheta;\n" +
        "        theta = lastTheta + midOmega * Time.deltaTime * step;\n" +
        "        theta = lastTheta + (lastOmega + newOmega) / 2 * Time.deltaTime * step;" +
        "        // 4. update omega\n\n" +
        "        omega = lastOmega - (g / length) * Mathf.Sin(lastTheta) * Time.deltaTime * step;\n" +
        "        omega = lastOmega - (g / length) * Mathf.Sin(midTheta) * Time.deltaTime * step;\n" +
        "        omega = lastOmega - (g / length) * Mathf.Sin((lastTheta + newTheta) / 2) * Time.deltaTime * step;" +
        "        // 5. move the object to the new postion\n" +
        "        deltaTheta = theta - lastTheta;\n" +
        "        SetPosition(deltaTheta);\n" +
        "    }\n\n" +
        "    // Start is called before the first frame update\n" +
        "    void Start()\n" +
        "    {\n" +
        "        // set the fixed position by gameobjtect\n" +
        "        fixedPos = fixP.transform.position;\n" +
        "        // set the initial position\n" +
        "        theta = theta * Mathf.Deg2Rad;\n" +
        "        SetPosition(theta);\n" +
        "        omega = 0.3f;\n" +
        "        count = 0;\n" +
        "    }\n\n" +
        "    // Update is called once per frame\n" +
        "    void FixedUpdate()\n" +
        "    {\n" +
        "        count++;\n" +
        "        if (count >= step)\n" +
        "        {\n" +
        "            UpdatePosition();\n" +
        "            count = 0;\n" +
        "        }\n" +
        "    }\n\n" +
        "    // calculate the position according to the pendulum length, angle and fixed point\n" +
        "    // angle: angle of rotation since last update, unit is Rad\n" +
        "    void SetPosition(float angle)\n" +
        "    {\n" +
        "        Vector3 pos = fixedPos;\n" +
        "        pos.x = fixedPos.x + length * Mathf.Sin(theta);\n" +
        "        pos.y = fixedPos.y - length * Mathf.Cos(theta);\n" +
        "        transform.position = pos;\n" +
        "        transform.Rotate(new Vector3(0, 0, angle * Mathf.Rad2Deg));\n" +
        "        // update position of the line\n" +
        "        Vector3 linePos = fixedPos;\n" +
        "        linePos.x = fixedPos.x + length * Mathf.Sin(theta) / 2;\n" +
        "        linePos.y = fixedPos.y - length * Mathf.Cos(theta) / 2;\n" +
        "        line.transform.position = linePos;\n" +
        "        line.transform.Rotate(new Vector3(0, 0, angle * Mathf.Rad2Deg));\n" +
        "    }\n" +
        "}\n";

    public static string getCodeFrame(string className)
    {
        return beforeClassName + className + afterClassName;
    }
}
