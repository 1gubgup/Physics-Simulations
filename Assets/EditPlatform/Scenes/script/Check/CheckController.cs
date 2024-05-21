using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckController : MonoBehaviour
{
    // UI
    public GameObject checkPanel;
    public GameObject waitPanel;
    public GameObject resultPanel;
    public InputField input;
    public GameObject comeBtn;
    public GameObject startButton;
    public GameObject endButton;
    public List<Toggle> toggles;
    public Controller gameController;
    public Text scoreText;
    public Dropdown dropdown;

    // check type
    private int currentCheck = 0;
    
    // name of object be checked
    private string objName;

    // check result
    private float score = 0;

    // check tools
    private CheckBase checkBounce;
    private CheckBase checkThrow;
    private CheckBase checkPEuler;
    private CheckBase checkPMidp;
    private CheckBase checkPTrpz;

    // check about report
    private bool reportFlag = false;
    private string scoreReview;

    private void Start()
    {
        // add listener for toggles
        int len = toggles.Count;
        for(int i = 0; i < len; i++)
        {
            int index = i;
            toggles[i].onValueChanged.AddListener(delegate (bool value)
            {
                onValueChanged(index, value);
            });
        }
        // init check tools
        checkBounce = transform.gameObject.AddComponent<CheckBase>();
        checkBounce.getCSVData("", "bounceEuler.csv");
        checkThrow = transform.gameObject.AddComponent<CheckBase>();
        checkThrow.getCSVData("throwEulerX.csv", "throwEulerY.csv");
        checkPEuler = transform.gameObject.AddComponent<CheckBase>();
        checkPEuler.getCSVData("pendulumEulerX.csv", "pendulumEulerY.csv");
        checkPMidp = transform.gameObject.AddComponent<CheckBase>();
        checkPMidp.getCSVData("pendulumMidpX.csv", "pendulumMidpY.csv");
        checkPTrpz = transform.gameObject.AddComponent<CheckBase>();
        checkPTrpz.getCSVData("pendulumTrpzX.csv", "pendulumTrpzY.csv");
    }

    public void SetReportFlag()
    {
        reportFlag = true;
        onCheckClick();
    }

    // 展开检查算法界面
    public void onCheckClick()
    {
        gameObject.SetActive(true);
        checkPanel.SetActive(true);
        waitPanel.SetActive(false);
        resultPanel.SetActive(false);
        comeBtn.GetComponent<Button>().interactable = false;
        gameController.updateStartEndBtn(false, false);
        gameController.setIfCheck(true);
    }

    // 开始检查算法实现
    public void onCheckConfirmClick()
    {
        score = 0;
        bool checkName = gameController.GetComponent<Controller>().checkObjectName(objName);
        if (checkName)
        {
            checkPanel.SetActive(false);
            waitPanel.SetActive(true);
            gameController.startButtonOnClick();
        }
    }

    // 取消检查
    public void onCheckCancleClick()
    {
        gameObject.SetActive(false);
        checkPanel.SetActive(true);
        waitPanel.SetActive(false);
        resultPanel.SetActive(false);
        comeBtn.GetComponent<Button>().interactable = true;
        gameController.updateStartEndBtn(true, false);
        gameController.setIfCheck(false);
        if (reportFlag) reportFlag = false;
    }

    // 对比计算结果
    public void onCheckFinish()
    {
        Dictionary<float, gameObjectData> data = gameController.getData(objName);
        switch (currentCheck)
        {
            // 自由落体 显式欧拉
            case 0:
                //checkBounce.setEnd();
                score = checkBounce.compare(data);
                break;
            // 抛物 显式欧拉
            case 1:
                score = checkThrow.compare(data);
                break;
            // 单摆 显式欧拉
            case 2:
                score = checkPEuler.compare(data);
                break;
            // 单摆 中点
            case 3:
                score = checkPMidp.compare(data);
                break;
            // 单摆 梯形
            case 4:
                score = checkPTrpz.compare(data);
                break;
            default:
                score = 0;
                break;
        }
        waitPanel.SetActive(false);
        resultPanel.SetActive(true);
        if (score > 0.9)
        {
            scoreText.text = Mathf.Round(score * 100) + "%\n" +
                "<color=#00FF16>实现正确</color>";
            scoreReview = "实现正确";
        }
        else
        {
            scoreText.text = Mathf.Round(score * 100) + "%\n" +
                "<color=#FFFF00>请检查算法实现\n或参数设置</color>";
            scoreReview = "算法实现或参数设置有误";
        }
        if (reportFlag)
        {
            GameObject.Find("ReportPanel").GetComponent<ReportController>().EditCheckComplete(Mathf.Round(score * 100), currentCheck, scoreReview);
            reportFlag = false;
        }
    }

    // 填写要检查的对象的name
    public void onNameAssign(string s)
    {
        if (s.Length == 0)
        {
            input.text = objName;
        }
        else
        {
            objName = s;
            gameController.GetComponent<Controller>().checkObjectName(s);
        }
    }

    // 选择下拉选框的物体名
    public void ChooseObject(int value)
    {
        if (value == -1) objName = "";
        else
        {
            objName = dropdown.options[value].text;
            gameController.GetComponent<Controller>().checkObjectName(objName);
        }
        Debug.Log(objName);
    }

    // 选择要检查的算法
    private void onValueChanged(int index, bool value)
    {
        if (value)
        {
            currentCheck = index;
        }
    }
}
