using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Chemix;
using UnityEngine.Serialization;
using System.Diagnostics.Tracing;

public class UI_Edit : MonoBehaviour
{
    public GameObject thisPanel;

    [FormerlySerializedAs("intivationPanel")]
    public GameObject invitationPanel;
    [FormerlySerializedAs("intivationPanelText")]
    public InputField invitationPanelText;
    public TMPro.TextMeshProUGUI judgeText;

    public InputField titleTextInput;
    public Transform BigContent;
    public Transform SmallContent;
    public GameObject stepPrefab;

    public UI_Step steps;

    //public float textOffset = -0.4f;
    public GameManager.ExperimentalSetup output;
    public GameObject titleText;
    public GameObject detailText;
    public GameObject objectList;

    public Slider cameraAngleSlider;
    public Slider cameraHeightSlider;
    public Slider cameraFOVSlider;
    public Michsky.UI.FieldCompleteMainMenu.SwitchManager conditionSwitch;
    public Slider rLightSlider;
    public Slider gLightSlider;
    public Slider bLightSlider;
    public Slider intensitySlider;
    public Light[] lights;

    private Dictionary<string, int> eventID;
    private List<string> eventIdList;
    private int score;
    private string review;
    private bool reportFlag = false;

    // Use this for initialization
    void Start()
    {

        thisPanel.SetActive(false);

        if (GM.GM_Core.instance == null)
            return;

        output = GM.GM_Core.instance.experimentalSetup;

        eventID = GM.GM_Core.instance.eventID;
        eventIdList = GM.GM_Core.instance.eventIdList;



        if (GM.GM_Core.instance.used)
        {
            Restore();
            GM.GM_Core.instance.used = false;
        }
    }

    public void SetReportFlag()
    {
        reportFlag = true;
        Complete();
    }

    public void Complete()
    {
        output.title = new GameManager.TextInfo();
        //output.title.color = titleText.GetComponent<Renderer>().material.GetColor("_Color");
        output.title.color = titleText.GetComponent<TextMesh>().color;
        output.title.position = titleText.transform.position;
        output.title.size = titleText.transform.localScale.x / titleText.GetComponent<Lab_Text>().srcScale.x;

        output.detail = new GameManager.TextInfo();
        //output.detail.color = detailText.GetComponent<Renderer>().material.GetColor("_Color");
        output.detail.color = detailText.GetComponent<TextMesh>().color;
        output.detail.position = detailText.transform.position;
        output.detail.size = detailText.transform.localScale.x / detailText.GetComponent<Lab_Text>().srcScale.x;

        output.instrumentInfos = new List<GameManager.InstrumentInfo>();
        for (int i = 0; i < objectList.transform.childCount; i++)
        {
            GameObject obj = objectList.transform.GetChild(i).gameObject;
            GameManager.InstrumentInfo info = new GameManager.InstrumentInfo();
            info.type = obj.name;
            info.quaternion = obj.transform.rotation;
            info.position = obj.transform.position;
            if (obj.GetComponent<Container>() != null)
            {
                Container c = obj.GetComponent<Container>();
                info.formula = c.typeName;
                info.mass = c.quantity;
            }
            output.instrumentInfos.Add(info);
        }

        output.taskFlow = new TaskFlow();
        output.taskFlow.title = steps.title;
        output.taskFlow.completeMessage = "恭喜你完成了本次实验";
        output.taskFlow.steps = new List<TaskFlow.Step>();
        for (int i = 0; i < steps.bigSteps.Count; i++)
        {
            TaskFlow.Step tf = new TaskFlow.Step();
            tf.detail = steps.bigSteps[i].GetComponent<UI_StepContent>().stepTitle;
            tf.substeps = new List<TaskFlow.Substep>();
            for (int j = 0; j < steps.smallSteps[i].Count; j++)
            {
                TaskFlow.Substep ss = new TaskFlow.Substep();
                ss.detail = steps.smallSteps[i][j].GetComponent<UI_StepContent>().stepTitle;
                ss.eventType = (TaskFlow.EventType)(int)steps.smallSteps[i][j].GetComponent<UI_StepContent>().tName;
                Debug.Log(steps.smallSteps[i][j].GetComponent<UI_StepContent>().eName);
                ss.taskEvent = (TaskFlow.TaskEvent)eventID[steps.smallSteps[i][j].GetComponent<UI_StepContent>().eName];
                tf.substeps.Add(ss);
            }
            output.taskFlow.steps.Add(tf);
        }

        output.envInfo = new GameManager.EnvironmentInfo();
        output.envInfo.cameraAngle = cameraAngleSlider.value;
        output.envInfo.cameraHeight = cameraHeightSlider.value;
        output.envInfo.cameraFOV = cameraFOVSlider.value;

        output.envInfo.lightInfo = new GameManager.LightInfo[3];

        for (int i = 0; i < lights.Length; i++)
        {
            GameManager.LightInfo li = new GameManager.LightInfo();
            li.color = lights[i].color; ;
            li.intensity = lights[i].intensity;
            li.position = lights[i].transform.position;
            li.rotation = lights[i].transform.rotation;
            output.envInfo.lightInfo[i] = li;
        }

        output.envInfo.useRoom = conditionSwitch.isOn;
        //Debug.Log(output.instrumentInfos.Count + " " + GM.GM_Core.instance.experimentalSetup.instrumentInfos.Count);

        //GM.GM_Core.instance.SwitchToScene("CustomLab");
        if (GM.GM_Core.instance == null)
        {
            Debug.Log(JudgeSetup(output));
        }

        if (GM.GM_Core.instance != null)
        {
            if (reportFlag)
            {
                invitationPanel.SetActive(true);
                judgeText.text = JudgeSetup(output);
                invitationPanel.transform.Find("InviteCode").gameObject.SetActive(false);
                return;
            }
            WWWForm form = new WWWForm();
            form.AddField("account", GM.GM_Core.instance.Account);
            form.AddField("password", GM.GM_Core.instance.Password);
            form.AddField("key", "scene");
            form.AddField("value", JsonUtility.ToJson(GM.GM_Core.instance.experimentalSetup));
            Chemix.Network.NetworkManager.Instance.Post(form, "scene/save", (success, reply) =>
            {
                string invite = reply.Detail;
                GM.GM_Core.instance.Invite = invite;
                invitationPanel.SetActive(true);
                invitationPanelText.text = Chemix.InviteUtility.InviteFrom(int.Parse(invite));
                Camera.main.GetComponent<Lab.Lab_Controller>().enabled = false;

                judgeText.text = JudgeSetup(output);
            }
                                                       );
        }
    }

    public enum MatchColorType
    {
        None,
        TongLeiSe,
        LinJinSe,
        LeiSiSe,
        DuiBiSe,
        HuBuSe,
    }

    private MatchColorType DoMatchColor(Color c1, Color c2)
    {
        float h1, h2;
        Color.RGBToHSV(c1, out h1, out _, out _);
        Color.RGBToHSV(c2, out h2, out _, out _);
        h1 *= 360;
        h2 *= 360;
        //Debug.Log($"Colors: {c1}[{h1}], {c2}[{h2}]");
        float delta = Mathf.Abs(h1 - h2);
        if (delta < 15)
        {
            return MatchColorType.TongLeiSe;
        }
        if (delta > 15 && delta < 45)
        {
            return MatchColorType.LinJinSe;
        }
        if (delta > 75 && delta < 105)
        {
            return MatchColorType.LeiSiSe;
        }
        if (delta > 105 && delta < 135)
        {
            return MatchColorType.DuiBiSe;
        }
        if (delta > 165 && delta < 195)
        {
            return MatchColorType.HuBuSe;
        }
        return MatchColorType.None;
    }

    private string ColorTypeToText(MatchColorType type)
    {
        string result;
        switch (type)
        {
            case MatchColorType.TongLeiSe:
                result = "同类色";
                break;
            case MatchColorType.LinJinSe:
                result = "邻近色";
                break;
            case MatchColorType.LeiSiSe:
                result = "类似色";
                break;
            case MatchColorType.DuiBiSe:
                result = "对比色";
                break;
            case MatchColorType.HuBuSe:
                result = "互补色";
                break;
            default:
                result = "无";
                break;
        }
        return result;
    }


    private string JudgeSetup(GameManager.ExperimentalSetup setup)
    {
        string[] uiLayout = { "字体大小、间距合理", "字体出现重叠，请重新调整位置" };
        string[] lightLayout = { "满足三点布光原则", "不满足三点布光原则，请重新调整光源布局" };
        string[] lightColor = { "", "不符色彩搭配理论，请遵从同类色/邻近色/类似色/对比色/互补色的设计原则，重新设置光源颜色" };

        int uiLayoutIndex = 0;
        int lightLayoutIndex = 0;
        int lightColorIndex = 0;

        // UI layout
        var b1 = titleText.GetComponent<BoxCollider>().bounds;
        var b2 = detailText.GetComponent<BoxCollider>().bounds;
        if (b1.Intersects(b2))
            uiLayoutIndex = 1;

        // Light layout
        bool[] hasThreeAngle = { false, false, false };
        foreach (var light in setup.envInfo.lightInfo)
        {
            var angle = light.rotation.eulerAngles.y;
            if (angle < 90 && angle > 0)
            {
                hasThreeAngle[0] = true;
            }
            else if (angle > 270 && angle < 360)
            {
                hasThreeAngle[1] = true;
            }
            else if (angle < 90 || angle > 270)
            {
                hasThreeAngle[2] = true;
            }
        }
        if (!(hasThreeAngle[0] && hasThreeAngle[1] && hasThreeAngle[2]))
        {
            lightLayoutIndex = 1;
        }

        // Light color
        var li = setup.envInfo.lightInfo;
        MatchColorType[] types = new MatchColorType[2];
        int typeIndex = 0;
        var type = DoMatchColor(li[0].color, li[1].color);
        if (type != MatchColorType.None)
        {
            types[typeIndex++] = type;
        }
        type = DoMatchColor(li[0].color, li[2].color);
        if (type != MatchColorType.None)
        {
            types[typeIndex++] = type;
        }
        type = DoMatchColor(li[1].color, li[2].color);
        if (typeIndex < 2 && type != MatchColorType.None)
        {
            types[typeIndex++] = type;
        }

        if (typeIndex < 2)
        {
            lightColorIndex = 1;
        }
        else
        {
            if (types[0] != types[1])
            {
                lightColor[0] = $"使用了{ColorTypeToText(types[0])}和{ColorTypeToText(types[1])}，符合色彩搭配理论";
            }
            else
            {
                lightColor[0] = $"使用了{ColorTypeToText(types[0])}，符合色彩搭配理论";
            }
        }

        string s1 = uiLayout[uiLayoutIndex];
        string s2 = lightLayout[lightLayoutIndex];
        string s3 = lightColor[lightColorIndex];
        score = 100 - uiLayoutIndex * 30 - lightLayoutIndex * 30 - lightColorIndex * 40;
        review = "【评估结果】:" +
        " UI布局：" + s1 +
        " 灯光布局：" + s2 +
        " 灯光颜色：" + s3;

        return string.Format("【评估结果】\n" +
        "UI布局：{0}\n" +
        "灯光布局：{1}\n" +
        "灯光颜色：{2}\n" +
        "得分：{3}/100", 
        s1, s2, s3, score);
    }

    public void Leave()
    {
        if (reportFlag)
        {
            invitationPanel.transform.Find("InviteCode").gameObject.SetActive(true);
            invitationPanel.SetActive(false);
            reportFlag = false;
            GameObject.Find("ReportPanel").GetComponent<ReportController>().EditCheckComplete(score, 5, review);
            return;
        }
        GM.GM_Core.instance.SwitchToScene("CustomLab");
    }

    public void Restore()
    {
        //titleText.GetComponent<Renderer>().material.SetColor("_Color", output.title.color);
        titleText.GetComponent<TextMesh>().color = output.title.color;
        titleText.transform.position = output.title.position;// - new Vector3(textOffset, 0, 0); ;
        titleText.transform.localScale = output.title.size * titleText.GetComponent<Lab_Text>().srcScale;

        //detailText.GetComponent<Renderer>().material.SetColor("_Color", output.title.color);
        detailText.GetComponent<TextMesh>().color = output.detail.color;
        detailText.transform.position = output.detail.position;
        detailText.transform.localScale = output.detail.size * detailText.GetComponent<Lab_Text>().srcScale;

        for (int i = 0; i < output.instrumentInfos.Count; i++)
        {
            GameObject obj = Instantiate(Resources.Load(output.instrumentInfos[i].type) as GameObject, objectList.transform);
            obj.name = output.instrumentInfos[i].type;
            obj.transform.rotation = output.instrumentInfos[i].quaternion;
            obj.transform.position = output.instrumentInfos[i].position;
            if (obj.GetComponent<Container>() != null)
            {
                Container c = obj.GetComponent<Container>();
                c.typeName = output.instrumentInfos[i].formula;
                c.quantity = output.instrumentInfos[i].mass;
            }
        }

        steps.title = output.taskFlow.title;
        titleTextInput.text = output.taskFlow.title;
        for (int i = 0; i < output.taskFlow.steps.Count; i++)
        {
            TaskFlow.Step tf = output.taskFlow.steps[i];
            GameObject newBigStep = steps.AddBigStep();//Instantiate(stepPrefab, BigContent.transform);
            newBigStep.GetComponent<UI_StepContent>().stepTitle = tf.detail;
            steps.curBigStepID = i + 1;
            //newBigStep.GetComponentInChildren<Text>().text = "流程" + (i + 1);
            //steps.bigSteps.Add(newBigStep);
            //steps.AddBigStep();

            //List<GameObject> smallSteps = new List<GameObject>();
            for (int j = 0; j < tf.substeps.Count; j++)
            {
                TaskFlow.Substep ss = tf.substeps[j];
                GameObject newSmallStep = steps.AddSmallStep(); //Instantiate(stepPrefab, SmallContent.transform);
                newSmallStep.GetComponentInChildren<Text>().text = "步骤" + (j + 1);
                newSmallStep.SetActive(false);
                //newSmallStep.GetComponent<UI_StepContent>().isBig = false;
                newSmallStep.GetComponent<UI_StepContent>().stepTitle = ss.detail;
                newSmallStep.GetComponent<UI_StepContent>().tName = (UI_StepContent.eventType)(int)ss.eventType;
                newSmallStep.GetComponent<UI_StepContent>().eName = eventIdList[(int)ss.taskEvent];
                //Debug.Log(newSmallStep.GetComponent<UI_StepContent>().eName);
                //smallSteps.Add(newSmallStep);
            }
            //steps.smallSteps.Add(smallSteps);
        }
        cameraFOVSlider.value = output.envInfo.cameraFOV;
        cameraFOVSlider.gameObject.GetComponent<UI.UI_Slider>().UpdateSliders();
        cameraAngleSlider.value = output.envInfo.cameraAngle;
        cameraAngleSlider.gameObject.GetComponent<UI.UI_Slider>().UpdateSliders();
        cameraHeightSlider.value = output.envInfo.cameraHeight;
        cameraAngleSlider.gameObject.GetComponent<UI.UI_Slider>().UpdateSliders();

        for (int i = 0; i < lights.Length; i++)
        {
            var li = output.envInfo.lightInfo[i];
            lights[i].color = li.color;
            lights[i].intensity = li.intensity;
            lights[i].transform.position = li.position;
            lights[i].transform.rotation = li.rotation;
        }

        cameraAngleSlider.gameObject.GetComponent<UI.UI_Slider>().UpdateSliders();
        conditionSwitch.isOn = output.envInfo.useRoom;
        conditionSwitch.initSwitch();

        steps.curBigStepID = 0;
        steps.curSmallStepID = 0;
    }
}
