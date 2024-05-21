using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowFrame
{
    public static string beforeClassName = "// 实验B：模拟抛物运动\n" +
            "// TODO:\n" +
            "// 1. 参考注释补全代码，用Explicit Euler方法模拟抛物运动\n" +
            "// 2. 设置不同的初速度，观察运动效果\n" +
            "// 3. 尝试不同大小的时间步长，通过修改step的值来控制步长\n" +
            "// 4. 不同时间步长之间进行对比，以及与解析式方式对比\n" +
            "class ";
    public static string afterClassName = " : MonoBehaviour\n" +
        "{\n" +
        "    // current position\n" +
        "    private float height;\n" +
        "    private float x;\n" +
        "    private float z;\n" +
         "    // current velocity\n" +
        "    private Vector3 v = new Vector3(15,0,0); // try different initial values\n" +
        "    private float g = 9.79f; // gravity\n" +
        "    private int step = 2; // You can change this!\n" +
        "    private int count; // used to control the frequency of updates\n\n" +
        "    // TODO: complete the function\n" +
        "    void UpdatePosition()\n" +
        "    {\n" +
        "        // 1. update position, move at speed of v for one time step \n\n" +
        //"        height = height + v.y * Time.deltaTime * step;" +
        //"        x = x + v.x * Time.deltaTime * step;" +
        //"        z = z + v.z * Time.deltaTime * step;" +
        "        // 2. calculate v in the next time step\n\n" +
        //"        v.y = v.y - g * Time.deltaTime * step;" +
        "        // 3. check whether reach the bottom\n\n" +
        //"        if (height <= transform.localScale.y/2)" +
        //"        {" +
        //"            v = Vector3.zero;" +
        //"        }" +
        "    }\n\n" +
        "    // Start is called before the first frame update\n" +
        "    void Start()\n" +
        "    {\n" +
        "        // set initial values\n" +
        "        height = transform.position.y;\n" +
        "        x = transform.position.x;\n" +
        "        z = transform.position.z;\n" +
        "        count = 0;\n" +
        "    }\n\n" +
        "    // Update is called once per frame\n" +
        "    void FixedUpdate()\n" +
        "    {\n" +
       "        count++;\n" +
        "        if (count >= step)\n" +
        "        {\n" +
        "            // calculate new position\n" +
        "            UpdatePosition();\n" +
        "            // set new position\n" +
        "            transform.position = new Vector3(x, height, z);\n" +
        "            count = 0;\n" +
        "        }\n" +
        "    }\n" +
        "}\n";

    public static string getCodeFrame(string className)
    {
        return beforeClassName + className + afterClassName;
    }
}
