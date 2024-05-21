using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace GM
{
    public class Report
    {
        public string Title
        {
            get;
            set;
        }

        public string Purpose
        {
            get;
            set;
        }

        public int Num
        {
            get;
            set;
        }

        public string listJson
        {
            get;
            set;
        }

        public string scoreJson
        {
            get;
            set;
        }
    }

    [Serializable]
    public class ReportPic
    {
        public string base64;
        public string des;
    }

    public class ScoreJson
    {
        public float score;
        public string method;
        public string review;
    }
}

public class ReportController : MonoBehaviour
{
    public Text PictureText;
    public Text ScoreText;
    public GameObject reportPrefab;
    public GameObject scrollContent;
    public GameObject ReportWait;

    private List<GM.PictureInfo> selectedPics;
    private int selectNum;
    private string title;
    private string purpose;

    private float Score;
    private string Method;
    private string Review;

    // Start is called before the first frame update
    void Start()
    {
        selectedPics = new List<GM.PictureInfo>();
        selectNum = 0;
        if (GM.GM_Core.instance.curSceneName == "EditPlatformScene")
        {
            title = "物理仿真算法在线编程实验";
            purpose = "本实验项目支持物理仿真算法编程这一数字媒体实践教学的难点内容。学生利用平台自主研发的在线编程环境，直观可视地编程实现交互媒体应用中的物理仿真技术，掌握常用的物理仿真数值求解算法及其特点。";
        }
        else if (GM.GM_Core.instance.curSceneName == "BuildExperiment")
        { 
            title = "交互媒体界面设计实验";
            purpose = "本实验项目支持交互媒体界面设计这一数字媒体实践教学的重点内容。学生利用实验平台的模型库和自主研发的内容编辑工具，直观地设计与开发交互媒体应用界面，掌握交互媒体界面的常用设计流程与设计原则。";
        }
        Score = 0;
        Method = "未采用任何评分方法";
        Review = "用户未进行评分";
    }

    public void SetTitle(string titleinput)
    {
        title = titleinput;
    }

    public void SetPurpose(string purposeinput)
    {
        purpose = purposeinput;
    }

    public void SelectPicture()
    {
        GM.GM_Core.instance.ReportOpenPic();
    }

    public void SelectComplete()
    {
        selectedPics = new List<GM.PictureInfo>(GM.GM_Core.instance.selectPics);
        selectNum = selectedPics.Count;
        if (selectNum == 0) PictureText.text = "<color=white>请上传可展示实验结果的截图</color>";
        else PictureText.text = "<color=#ffff00>获取到<color=#00ff00> " + selectNum + " </color>张图片数据</color>";

        for (int i = scrollContent.transform.childCount - 1; i >= 0; i--) Destroy(scrollContent.transform.GetChild(i).gameObject);      
        for(int i = 1; i <= selectNum; ++i)
        {
            GameObject g = Instantiate(reportPrefab, scrollContent.transform);
            g.transform.Find("Number").GetComponent<Text>().text = i.ToString();
            g.GetComponentInChildren<RawImage>().texture = LoadImage.GetTextureByByte(selectedPics[i - 1].Byte);
            g.transform.Find("description").GetComponent<Text>().text = selectedPics[i - 1].Des;
        }
        Debug.Log(selectNum);
    }

    public void EditCheckComplete(float score,int checkType,string review)
    {
        Score = score;
        Review = review;
        switch (checkType)
        {
            case 0:Method = "自由落体显式欧拉";break;
            case 1:Method = "抛物显式欧拉";break;
            case 2:Method = "单摆显式欧拉";break;
            case 3:Method = "单摆中点法";break;
            case 4:Method = "单摆梯形法";break;
            case 5:Method = "场景设计评估";break;
            default:Method = "未知方法";break;
        }
        ScoreText.text = "得分:" + Score + "/100   评分方式:" + Method;
    }

    public void Confirm()
    {
        GM.Report report = new GM.Report();

        report.Title = title;
        report.Purpose = purpose;
        report.Num = selectNum;

        List<GM.ReportPic> reportlist = new List<GM.ReportPic>();
        for (int i = 0; i < selectNum; ++i)
        {
            GM.ReportPic reportPic = new GM.ReportPic();
            reportPic.base64 = Convert.ToBase64String(selectedPics[i].Byte);
            //reportPic.base64 = "testbase64";
            reportPic.des = selectedPics[i].Des;
            reportlist.Add(reportPic);
        }
        report.listJson = SerializeList.ListToJson<GM.ReportPic>(reportlist);

        GM.ScoreJson scoreJson = new GM.ScoreJson();
        scoreJson.score = Score;
        scoreJson.method = Method;
        scoreJson.review = Review;
        report.scoreJson = JsonUtility.ToJson(scoreJson);

        ReportWait.SetActive(true);

        GM.GM_Core.instance.Report(report);
        Close();
    }

    public void showMySelf()
    {
        this.gameObject.SetActive(true);
    }

    public void Close()
    {
        Score = 0;
        Method = "未采用任何评分方法";
        Review = "用户未进行评分";
        ScoreText.text = "请点击\"系统评分\"按钮对结果进行评分";
        this.gameObject.SetActive(false);
    }
}
