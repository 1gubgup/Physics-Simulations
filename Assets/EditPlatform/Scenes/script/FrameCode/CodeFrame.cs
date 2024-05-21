using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CodeFrame
{
    public static string beforeClassName = "class ";
    public static string afterClassName = " : MonoBehaviour\n" +
        "{\n" +
        "    // Start is called before the first frame update\n" +
        "    void Start()\n" +
        "    {\n\n" +
        "    }\n\n" +
        "    // Update is called once per frame\n" +
        "    void FixedUpdate()\n" +
        "    {\n\n" +
        "    }\n" +
        "}\n";

    public static string getCodeFrame(string className)
    {
        return beforeClassName + className + afterClassName;
    }
}
