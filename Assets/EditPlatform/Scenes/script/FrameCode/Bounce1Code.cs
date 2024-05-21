using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Bounce1Code
{
    public static string beforeClassName = "// 实验A：模拟自由落体运动-解析式方法\n" +
        "// 通过解析式模拟物体自由落体并反弹的往复运动\n" +
        "// 可以用来与Explicit Euler方式的模拟进行对比\n" +
        "class ";
    public static string afterClassName = " : MonoBehaviour\n" +
        "{\n" +
        "    private double g = 1.0; // gravity\n" +
        "    private double h0; // initial position\n" +
        "    private double height; // current position\n" +
        "    private double t; // elapsed time\n" +
        "    private float x;\n" +
        "    private float z;\n" +
        "    private double v0; // initial velocity\n" +
        "    private double v; // current velocity\n\n" +
        "    // Start is called before the first frame update\n" +
        "    void Start()\n" +
        "    {\n" +
        "        // set initial values\n" +
        "        height = transform.position.y;\n" +
        "        h0 = height;\n" +
        "        x = transform.position.x;\n" +
        "        z = transform.position.z;\n" +
        "        t = 0;\n" +
        "        v0 = 0;\n" +
        "        v = 0;\n" +
        "    }\n\n" +
        "    // Update is called once per frame\n" +
        "    void FixedUpdate()\n" +
        "    {\n" +
        "        // update elapsed time\n" +
        "        t = t + Time.deltaTime;\n" +
        "        // calculate new position\n" +
        "        UpdateHeight();\n" +
        "        // set new position\n" +
        "        transform.position = new Vector3(x, (float)height, z);\n" +
        
        "    }\n\n" +
        "    // Analytical Solution\n" +
        "    void UpdateHeight()\n" +
        "    {\n" +
        "        // 1. calculate displacement\n" +
        "        double delta = v0 * t - g * t * t / 2;\n" +
        "        // 2. calculate current height with initial height and displacement\n" +
        "        height = h0 + delta;\n" +
        "        // 3. calculate current velocity\n" +
        "        v = v0 - g * t;\n" +
        "        // 4. change direction if needed, reset initial values\n" +
        "        //     case 1: reach the bottom\n" +
        "        if (height <= transform.localScale.y/2)\n" +
        "        {\n" +
        "            h0 = transform.localScale.y/2;\n" +
        "            v0 = g * t;\n" +
        "            t = 0;\n" +
        "        }\n" +
        "        //     case 2: reach the peak\n" +
        "        else if (v <= 0 && v0 > 0)\n" +
        "        {\n" +
        "            h0 = height;\n" +
        "            v0 = 0;\n" +
        "            t = 0;\n" +
        "        }\n" +
        "    }\n}";

    public static string getCodeFrame(string className)
    {
        return beforeClassName + className + afterClassName;
    }
}
