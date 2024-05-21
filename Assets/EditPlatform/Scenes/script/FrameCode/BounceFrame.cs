using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BounceFrame
{
    public static string beforeClassName = "// 实验A：模拟自由落体运动-Explicit Euler方法\n" +
        "// TODO: \n" +
        "// 1. 参考注释补全代码，用Explicit Euler方法模拟自由落体和反弹运动\n" +
        "// 2. 尝试不同大小的时间步长，通过修改step的值来控制步长\n" +
        "// 3. 不同时间步长之间进行对比，以及与解析式方式对比\n" +
        "class ";
    public static string afterClassName = " : MonoBehaviour\n" +
        "{\n" +
        "    private double g = 9.79;\n" +
        "    private double height;\n" +
        "    private float x;\n" +
        "    private float z;\n" +
        "    private double v;\n\n" +
        "    private int step = 2; // You can change this!\n" +
        "    private int count; // used to control the frequency of updates\n\n" +
        "    // TODO: complete the function with Explicit Euler method\n" +
        "    void UpdateHeight()\n" +
        "    {\n" +
        "        // 1. update height, move at speed of v for one time step\n\n" +
        //"        height = height + v * Time.deltaTime * step;\n" +
        "        // 2. calculate v in the next time step\n\n" +
        //"        v = v - g * Time.deltaTime * step;\n" +
        "        // 3. change direction if needed, reset initial values\n\n" +
        //"        if (height <= transform.localScale.y/2)\n" +
        //"        {\n" +
        //"            v = -v;\n" +
        //"        }\n" +
        "    }\n\n" +
        "    // Start is called before the first frame update\n" +
        "    void Start()\n" +
        "    {\n" +
        "        // set initial values\n" +
        "        height = transform.position.y;\n" +
        "        x = transform.position.x;\n" +
        "        z = transform.position.z;\n" +
        "        v = 0;\n" +
        "        count = 0;\n" +
        "    }\n\n" +
        "    // Update is called once per frame\n" +
        "    void FixedUpdate()\n" +
        "    {\n" +
        "        count++;\n" +
        "        if (count >= step)\n" +
        "        {\n" +
        "            // calculate new position\n" +
        "            UpdateHeight();\n" +
        "            // set new position\n" +
        "            transform.position = new Vector3(x, (float)height, z);\n" +
        "            count = 0;\n" +
        "        }\n" +
        "    }\n\n" +
        "}\n";

    public static string getCodeFrame(string className)
    {
        return beforeClassName + className + afterClassName;
    }
}
