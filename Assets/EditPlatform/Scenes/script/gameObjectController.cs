using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interpreter_Execute;
using Interpreter_Basic;

public class gameObjectController : MonoBehaviour
{
    bool ifStart = false;
    bool ifFirstStart = false;
    Code_exe code;
    GameObject gameController;
    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (ifFirstStart)
        {
            ifFirstStart = false;
            ifStart = true;
            try
            {
                code.exe_start();
            }
            catch (MyException e)
            {
                gameController.GetComponent<Controller>().exceptionDeliver(e.getInformation());
            }
        }
        else if (ifStart)
        {
            try
            {
                code.exe_update();
            }
            catch (MyException e)
            {
                gameController.GetComponent<Controller>().exceptionDeliver(e.getInformation());
                ifStart = false;
            }
        }
    }

    public void StartFlag(Code_exe c)
    {
        code = c;
        ifFirstStart = true;
        ifStart = false;
    }

    public void EndFlag()
    {
        ifStart = false;
        ifFirstStart = false;
    }
}
