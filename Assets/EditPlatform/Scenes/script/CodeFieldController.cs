using System.Collections;
using System.Collections.Generic;
using Interpreter_Basic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public enum CodeType
{
    BOUNCE, // 自由落体反弹的模板
    BOUNCE_SOLUTION, // 自由落体反弹的解析式实现
    BOUNCE1_SOLUTION, // 自由落体反弹的解析式实现 - 微重力
    THROW, // 平抛的模板
    THROW_SOLUTION, // 平抛的解析式实现
    THROW1_SOLUTION, // 平抛的解析式实现 - 微重力
    PENDULUM, // 单摆的模板
    PENDULUM_SOLUTION, // 单摆的欧拉实现
    PENDULUM1_SOLUTION, // 单摆的欧拉实现 - 微重力
    DEFAULT // 空文件模板
}

public class CodeFieldController : MonoBehaviour
{
    //与jslib的通信函数，详见Plugins文件夹下的.jslib文件
    [DllImport("__Internal")]
    private static extern void ClickSelectFileFoldBtn();
#if UNITY_WEBGL && !UNITY_EDITOR

    [DllImport("__Internal")]
    private static extern void SendCode(string str);
    [DllImport("__Internal")]
    private static extern void SendCodeName(string str);

#endif
    public GameObject EnterCodeName;
    public GameObject SaveCode;
    public GameObject Menu;
    public InputField CodeField, PathField, NameField;
    private GameObject gameController;
    private GameObject exceptionfield;
    private CodeType newCodeType = CodeType.DEFAULT; // 要创建的新文件的类型
    private string importcode = "";

    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController");
        exceptionfield = GameObject.FindGameObjectWithTag("EditPlatformCanvas").transform.Find("exceptionField").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetImportcode(string code)
    {
        importcode = code;
    }

    public void buttonOnClick()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        gameController.GetComponent<Controller>().codeOnEdit = gameController.GetComponent<Controller>().getCodeName(gameObject.transform);
        SendCodeName(gameController.GetComponent<Controller>().getCodeName(gameObject.transform));
        SendCode(gameController.GetComponent<Controller>().getCode(gameObject.transform));
#endif
        gameController.GetComponent<Controller>().codeFieldDown();
        gameController.GetComponent<Controller>().codeFieldUp(transform);
    }

    public void deleteButtonOnClick()
    {
        gameController.GetComponent<Controller>().deleteButtonOnClick(gameObject.transform);
    }

    public void RenameOnClick()
    {
        EnterCodeName.SetActive(true);
    }

    public void CloseOnClick()
    {
        Menu.SetActive(false);
    }

    public void EnterNameConfirm(InputField i)
    {
        int ret = gameController.GetComponent<Controller>().setCodeName(gameObject.transform, i.text, newCodeType, importcode);
        if (ret == 0) EnterCodeName.SetActive(false);
    }

    public void EnterNameCancel()
    {
        if (CodeField.text != null && CodeField.text != "")
        {
            EnterCodeName.SetActive(false);
        }
        else
        {
            deleteButtonOnClick();
        }
    }

    public void OnCodeTextAssign()
    {
        gameController.GetComponent<Controller>().CodeTextAssign(gameObject.transform);
    }

    // 点击menu中的save按钮
    public void SaveOnClick()
    {
        string name = gameController.GetComponent<Controller>().getCodeName(transform);
        NameField.text = name + ".cs";
#if UNITY_WEBGL && !UNITY_EDITOR
        gameController.GetComponent<Controller>().SaveFile("", NameField.text, CodeField.text);
#endif
#if UNITY_EDITOR
        SaveCode.SetActive(true);
#endif
    }

    // 确认保存
    public void SaveConfirm()
    {
        bool result = gameController.GetComponent<Controller>().SaveFile(PathField.text, NameField.text, CodeField.text);
        SaveCode.SetActive(!result);
    }

    // 取消保存
    public void SaveCancel()
    {
        SaveCode.SetActive(false);
    }

    public void OnBounceCheckChanged(bool value)
    {
        if (value)
        {
            newCodeType = CodeType.BOUNCE;
        }
    }

    public void OnSolutionCheckChanged(bool value)
    {
        if (value)
        {
            newCodeType = CodeType.BOUNCE_SOLUTION;
        }
    }

    public void OnThrowCheckChanged(bool value)
    {
        if (value)
        {
            newCodeType = CodeType.THROW;
        }
    }

    public void OnSolutionThrowChanged(bool value)
    {
        if (value)
        {
            newCodeType = CodeType.THROW_SOLUTION;
        }
    }

    public void OnPendulumCheckChanged(bool value)
    {
        if (value)
        {
            newCodeType = CodeType.PENDULUM;
        }
    }

    public void OnDefaultCheckChanged(bool value)
    {
        if (value)
        {
            newCodeType = CodeType.DEFAULT;
        }
    }
}
