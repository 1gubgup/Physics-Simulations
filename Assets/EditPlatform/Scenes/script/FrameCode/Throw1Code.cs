using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw1Code
{
    public static string beforeClassName = "// 实验B：模拟抛物运动\n" +
            "// 通过解析式模拟抛物运动\n" +
            "// 可以用来与Explicit Euler方式的模拟进行对比\n" +
            "class ";
    public static string afterClassName = " : MonoBehaviour\n" +
        "{\n" +
        "    // initial position \n" +
        "    private float h0; \n" +
        "    private float x0;\n" +
        "    private float z0;\n" +
        "    // current position\n" +
        "    private float height;\n" +
        "    private float x;\n" +
        "    private float z;\n" +
        "    // initial velocity, try different values\n" +
        "    private Vector3 v0 = new Vector3(15, 0, 0);\n" +
        "    // current velocity\n" +
        "    private Vector3 v;\n" +
        "    private float t; // elapsed time\n" +
        "    private float g = 1.0f; // gravity\n\n" +
        "    // Start is called before the first frame update\n" +
        "    void Start()\n" +
        "    {\n" +
        "        // set initial values\n" +
        "        height = transform.position.y;\n" +
        "        h0 = height;\n" +
        "        x = transform.position.x;\n" +
        "        x0 = x;\n" +
        "        z = transform.position.z;\n" +
        "        z0 = z;\n" +
        "        t = 0;\n" +
        "        v = Vector3.zero;\n"+
        "    }\n\n" +
        "    // Update is called once per frame\n" +
        "    void FixedUpdate()\n" +
        "    {\n" +
        "        // update elapsed time\n" +
        "        t = t + Time.deltaTime;\n" +
        "        // calculate new position\n" +
        "        UpdateHeight();\n" +
        "        // set new position\n" +
        "        transform.position = new Vector3(x, height, z);\n" +
        "    }\n\n" +
        "    // Analytical Solution\n" +
        "    void UpdateHeight()\n" +
        "    {\n" +
        "        // 1. calculate displacement\n" +
        "        float deltay = v0.y * t - g * t * t / 2;\n" +
        "        float deltax = v0.x * t;\n" +
        "        float deltaz = v0.z * t;\n" +
        "        // 2. calculate current position with initial position and displacement \n" +
        "        height = h0 + deltay;\n" +
        "        x = x0 + deltax;\n" +
        "        z = z0 + deltaz;\n" +
        "        // 3. calculate current velocity, v only changes in y-axis\n" +
        "        v.y = v0.y - g * t;\n" +
        "        // 4. check whether reach the bottom, stay static\n" +
        "        if (height <= transform.localScale.y/2)\n" +
        "        {\n" +
        "            h0 = transform.localScale.y/2;\n" +
        "            x0 = x;\n" +
        "            z0 = z;\n" +
        "            v0 = Vector3.zero;\n" +
        "            t = 0;\n" +
        "        }\n" +
        "    }\n" +
        "}\n";

    public static string getCodeFrame(string className)
    {
        return beforeClassName + className + afterClassName;
    }
}
