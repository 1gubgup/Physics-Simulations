using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.Windows;
using Interpreter_Basic;
using Interpreter_Execute;
using Interpreter_Semantic_Analyzer;
using Interpreter_Parser;
using System.Runtime.InteropServices;
using Chemix;
using UnityEngine.SceneManagement;
//using System;

public class Code
{
    public string codeName;
    public string textForCode;
    private Parser p;
    private AST node;


    public Code(string cN, string cT)
    {
        codeName = cN;
        textForCode = cT;
    }

    public void setNode(Dictionary<string, HaHaObject> d)
    {
        p = new Parser(textForCode, d);
        node = p.getAST();
    }

    public AST getNode()
    {
        return node;
    }
}

public class Code_exe
{
    private Execute execute;
    private string codeName;

    public Code_exe(string n)
    {
        codeName = n;
    }

    public void exe_start()
    {
        execute.excute_start();
    }

    public void exe_update()
    {
        execute.excute_update();
    }

    public void exe_init(HaHaObject g, Dictionary<string, HaHaObject> d, AST node)
    {
        g.gameObject_init();
        execute = new Execute(node, g, codeName, d);
    }
}

public class HaHaObject
{
    public string objectName;
    public Code_exe code_exe;
    public GameObject gameObject;
    public Vector3 init_pos = new Vector3();
    public Vector3 init_rot = new Vector3();
    public Vector3 init_scale = new Vector3();
    public Dictionary<float, gameObjectData> dataDict = new Dictionary<float, gameObjectData>();
    public Transform chartField;
    public bool tracing; // draw the motion trace or not
    public LineRenderer lineRenderer;
    public string type;
    public int material;

    public HaHaObject(string oN)
    {
        objectName = oN;
    }

    public HaHaObject(string oN, GameObject obj)
    {
        objectName = oN;
        gameObject = obj;
        update_init_pos();
        update_init_rot();
        update_init_scale();
        // 初始化绘制运动轨迹的属性
        //tracing = false;
        tracing = true;
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.7f;
        lineRenderer.endWidth = 0.7f;
        lineRenderer.startColor = new Color(0, 0, 1);
        lineRenderer.endColor = new Color(1, 0, 1);
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    }

    public void gameObject_init()
    {
        gameObject.transform.position = init_pos;
        gameObject.transform.eulerAngles = init_rot;
        gameObject.transform.localScale = init_scale;
    }

    public void update_gameObject_pos()
    {
        gameObject.transform.position = init_pos;
    }

    public void update_gameObject_rot()
    {
        gameObject.transform.eulerAngles = init_rot;
    }

    public void update_gameObject_scale()
    {
        gameObject.transform.localScale = init_scale;
    }

    public void update_init_pos()
    {
        init_pos = gameObject.transform.position;
    }

    public void update_init_rot()
    {
        init_rot = gameObject.transform.eulerAngles;
    }

    public void update_init_scale()
    {
        init_scale = gameObject.transform.localScale;
    }

    public void addTrace(Vector3 pos)
    {
        if (!tracing)
        {
            return;
        }
        int count = lineRenderer.positionCount;
        if(count >= 1)
        {
            // 和最后一个点对比
            Vector3 lastPos = lineRenderer.GetPosition(count - 1);
            // 停在统一个点
            if ((pos - lastPos).sqrMagnitude <= 0.0001)
            {
                return;
            }
        }
        lineRenderer.positionCount = count + 1;
        lineRenderer.SetPosition(count, pos);
    }

    public void clearTrace()
    {
        lineRenderer.positionCount = 0;
    }
}

public class gameObjectData
{
    public Vector3 pos;
    public Vector3 rot;
    public Vector3 scale;

    public gameObjectData(Vector3 p, Vector3 r, Vector3 s)
    {
        pos = p;
        rot = r;
        scale = s;
    }
}


public class Controller : MonoBehaviour
{
    public GameManager.ExperimentalScene savedScene;
    //public GM.GM_Core gm;

    public GameObject codePart;
    public GameObject objectPartContent;
    public GameObject chartPartContent;
    public GameObject addButton_code;
    public GameObject addButton_object;
    public GameObject startButton;
    public GameObject endButton;
    public GameObject checkButton;
    public Object codeFieldPrefab;
    public Object sphereObjectFieldPrefab;
    public Object spherePrefab;
    public Object chartField;
    public Object cubeObjectFieldPrefab;
    public Object cubePrefab;
    public GameObject exceptionField;
    public Text exceptionText;
    public GameObject checkPanel;
    public Dropdown dropdown;
    public GameObject ReportWait;
    public GameObject isQuitUI;

    //public forTest1 theTest1;
    //public forTest2 theTest2;
    //public forTest3 theTest3;

    public Text test_text1;
    public Text test_text2;
    public Text test_text3;

    //private bool t1a = false;
    //private bool t2a = false;
    //private bool t3a = false;

    private bool ifStart = false;

    private Dictionary<Transform, Code> codeDict = new Dictionary<Transform, Code>();

    private Dictionary<string, Code> nameCodeDict = new Dictionary<string, Code>();

    private Dictionary<Transform, HaHaObject> objectDict = new Dictionary<Transform, HaHaObject>();

    private Dictionary<string, HaHaObject> nameObjectDict = new Dictionary<string, HaHaObject>();

    private Transform currentField;

    private float delta_y = 3; // 被选中的tab比未选中的tab的高度

    private float field_num = 0; // 代码文件数量

    private float current_width = 120; // 代码区域顶部按钮的宽度

    private float init_width = 120;

    //private bool record_flag = false;

    private float time_temp;

    private float start_time;

    private float chart_time_step = 0.2f;

    private bool ifCheck = false; // 是否是检查状态

    private int checkCount = 0; // 计数检查时统计的数据个数

    private string check_code_name = "";

    public string codeOnEdit;

    //与jslib的通信函数，详见Plugins文件夹下的.jslib文件
    //[DllImport("__Internal")]
    //private static extern void JSInit();
    [DllImport("__Internal")]
    private static extern void ClickSaveScript(string filename, string text);

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        //Init the env of JS communication
        //JSInit();
#endif

        //if (GM.GM_Core.instance.used)
        //{
        //    savedScene = GM.GM_Core.instance.experimentalScene;
        //    Restore("");
        //    GM.GM_Core.instance.used = false;
        //    return;
        //}


        Transform test1_transform = AddButtonOnClickForInit();
        setCodeName(test1_transform, "Throw", CodeType.THROW_SOLUTION, ""); 

        Transform test2_transform = AddButtonOnClickForInit();
        setCodeName(test2_transform, "SpaceThrow", CodeType.THROW1_SOLUTION, "");

        Transform test3_transform = AddButtonOnClickForInit();
        setCodeName(test3_transform, "Bounce", CodeType.BOUNCE_SOLUTION, ""); 

        Transform test4_transform = AddButtonOnClickForInit();
        setCodeName(test4_transform, "SpaceBounce", CodeType.BOUNCE1_SOLUTION, "");

        Transform test5_transform = AddButtonOnClickForInit();
        setCodeName(test5_transform, "Pendulum", CodeType.PENDULUM_SOLUTION, ""); 

        Transform test6_transform = AddButtonOnClickForInit();
        setCodeName(test6_transform, "SpacePendulum", CodeType.PENDULUM1_SOLUTION, "");

        Transform test7_transform = AddButtonOnClickForInit();
        setCodeName(test7_transform, "Default", CodeType.DEFAULT, "");


        // Add sphere
        Transform sphere = AddSphereButtonObject();
        SetupExperimentObject(sphere, "DefaultSphere", "Throw", new Vector3(-70, 50, 600), new Vector3(10, 10, 10), true);

        // Add sphere1
        Transform sphere1 = AddSphereButtonObject();
        SetupExperimentObject(sphere1, "SpaceSphere", "SpaceThrow", new Vector3(-20, 50, 600), new Vector3(10, 10, 10), true);
        // change material
        Material spheremat2 = sphere1.GetComponent<objectFieldController>().mat2;
        sphere1.GetChild(0).Find("Mat2").GetComponent<Toggle>().isOn = true;
        changeMat(sphere1, spheremat2, 2);
        Sprite shpereimg2 = sphere1.GetComponent<objectFieldController>().img2;
        sphere1.GetComponent<objectFieldController>().image.sprite = shpereimg2;


        // Add cube
        Transform cube = AddCubeButtonObject();
        SetupExperimentObject(cube, "DefaultCube", "Bounce", new Vector3(150, 50, 530), new Vector3(10, 10, 10), true);
        // change material
        Material cubemat2 = cube.GetComponent<objectFieldController>().mat2;
        cube.GetChild(0).Find("Mat2").GetComponent<Toggle>().isOn = true;
        changeMat(cube, cubemat2, 2);
        Sprite cubeimg2 = cube.GetComponent<objectFieldController>().img2;
        cube.GetComponent<objectFieldController>().image.sprite = cubeimg2;

        // Add cube1
        Transform cube1 = AddCubeButtonObject();
        SetupExperimentObject(cube1, "SpaceCube", "SpaceBounce", new Vector3(150, 50, 470), new Vector3(10, 10, 10), true);
        // change material
        Material cubemat3 = cube1.GetComponent<objectFieldController>().mat3;
        cube1.GetChild(0).Find("Mat3").GetComponent<Toggle>().isOn = true;
        changeMat(cube1, cubemat3, 3);
        Sprite cubeimg3 = cube1.GetComponent<objectFieldController>().img3;
        cube1.GetComponent<objectFieldController>().image.sprite = cubeimg3;


        // Add pendulum objects
        Transform fixP = AddCubeButtonObject();
        //SetupExperimentObject(fixP, "fixP", "", new Vector3(40, 50, 400), new Vector3(2, 2, 2), false);

        // set name
        fixP.GetChild(0).Find("name").GetComponentInChildren<InputField>().text = "fixP";
        objectNameAssign(fixP, "fixP");
        // set position
        fixP.GetChild(0).Find("PosX").GetComponentInChildren<InputField>().text = "40";
        PosXAssign(fixP, 40);
        fixP.GetChild(0).Find("PosY").GetComponentInChildren<InputField>().text = "50";
        PosYAssign(fixP, 50);
        fixP.GetChild(0).Find("PosZ").GetComponentInChildren<InputField>().text = "400";
        PosZAssign(fixP, 400);
        // set scale
        fixP.GetChild(0).Find("ScaX").GetComponentInChildren<InputField>().text = "2";
        ScaleXAssign(fixP, 2);
        fixP.GetChild(0).Find("SczY").GetComponentInChildren<InputField>().text = "2";
        ScaleYAssign(fixP, 2);
        fixP.GetChild(0).Find("SczZ").GetComponentInChildren<InputField>().text = "2";
        ScaleZAssign(fixP, 2);
        // change material
        Material fixPmat3 = fixP.GetComponent<objectFieldController>().mat3;
        fixP.GetChild(0).Find("Mat3").GetComponent<Toggle>().isOn = true;
        changeMat(fixP, fixPmat3, 3);
        Sprite fixPimg3 = fixP.GetComponent<objectFieldController>().img3;
        fixP.GetComponent<objectFieldController>().image.sprite = fixPimg3;
        // draw track by default
        fixP.GetChild(0).Find("CheckDraw").GetComponent<Toggle>().isOn = false;
        setObjTracing(fixP, false);

        Transform line = AddSphereButtonObject();
        //SetupExperimentObject(line, "Line", "Pendulum", new Vector3(40, 25, 400), new Vector3(6, 6, 6), true);

        // set name
        line.GetChild(0).Find("name").GetComponentInChildren<InputField>().text = "line";
        objectNameAssign(line, "line");
        // set class name
        line.GetChild(0).Find("class name").GetComponentInChildren<InputField>().text = "Pendulum";
        codeNameAssign(line, "Pendulum");
        // set scale
        line.GetChild(0).Find("ScaX").GetComponentInChildren<InputField>().text = "6";
        ScaleXAssign(line, 6);
        line.GetChild(0).Find("SczY").GetComponentInChildren<InputField>().text = "6";
        ScaleYAssign(line, 6);
        line.GetChild(0).Find("SczZ").GetComponentInChildren<InputField>().text = "6";
        ScaleZAssign(line, 6);
        // set position
        line.GetChild(0).Find("PosX").GetComponentInChildren<InputField>().text = "40";
        PosXAssign(line, 40);
        line.GetChild(0).Find("PosY").GetComponentInChildren<InputField>().text = "25";
        PosYAssign(line, 25);
        line.GetChild(0).Find("PosZ").GetComponentInChildren<InputField>().text = "400";
        PosZAssign(line, 400);
        // draw track by default
        line.GetChild(0).Find("CheckDraw").GetComponent<Toggle>().isOn = true;
        setObjTracing(line, true);


        // Add pendulum objects
        Transform fixP1 = AddCubeButtonObject();
        //SetupExperimentObject(fixP1, "SpacefixP", "", new Vector3(-40, 50, 400), new Vector3(2, 2, 2), false);

        // set name
        fixP1.GetChild(0).Find("name").GetComponentInChildren<InputField>().text = "SpacefixP";
        objectNameAssign(fixP1, "SpacefixP");
        // set position
        fixP1.GetChild(0).Find("PosX").GetComponentInChildren<InputField>().text = "-40";
        PosXAssign(fixP1, -40);
        fixP1.GetChild(0).Find("PosY").GetComponentInChildren<InputField>().text = "50";
        PosYAssign(fixP1, 50);
        fixP1.GetChild(0).Find("PosZ").GetComponentInChildren<InputField>().text = "400";
        PosZAssign(fixP1, 400);
        // set scale
        fixP1.GetChild(0).Find("ScaX").GetComponentInChildren<InputField>().text = "2";
        ScaleXAssign(fixP1, 2);
        fixP1.GetChild(0).Find("SczY").GetComponentInChildren<InputField>().text = "2";
        ScaleYAssign(fixP1, 2);
        fixP1.GetChild(0).Find("SczZ").GetComponentInChildren<InputField>().text = "2";
        ScaleZAssign(fixP1, 2);
        // change material
        Material fix1Pmat3 = fixP1.GetComponent<objectFieldController>().mat3;
        fixP1.GetChild(0).Find("Mat3").GetComponent<Toggle>().isOn = true;
        changeMat(fixP1, fix1Pmat3, 3);
        Sprite fixP1img3 = fixP1.GetComponent<objectFieldController>().img3;
        fixP1.GetComponent<objectFieldController>().image.sprite = fixP1img3;
        // draw track by default
        fixP1.GetChild(0).Find("CheckDraw").GetComponent<Toggle>().isOn = false;
        setObjTracing(fixP1, false);

        Transform line1 = AddSphereButtonObject();
        //SetupExperimentObject(line1, "SpaceLine", "SpacePendulum", new Vector3(-40, 25, 400), new Vector3(6, 6, 6), true);

        // set name
        line1.GetChild(0).Find("name").GetComponentInChildren<InputField>().text = "SpaceLine";
        objectNameAssign(line1, "SpaceLine");
        // set class name
        line1.GetChild(0).Find("class name").GetComponentInChildren<InputField>().text = "SpacePendulum";
        codeNameAssign(line1, "SpacePendulum");
        // set scale
        line1.GetChild(0).Find("ScaX").GetComponentInChildren<InputField>().text = "6";
        ScaleXAssign(line1, 6);
        line1.GetChild(0).Find("SczY").GetComponentInChildren<InputField>().text = "6";
        ScaleYAssign(line1, 6);
        line1.GetChild(0).Find("SczZ").GetComponentInChildren<InputField>().text = "6";
        ScaleZAssign(line1, 6);
        // set position
        line1.GetChild(0).Find("PosX").GetComponentInChildren<InputField>().text = "-40";
        PosXAssign(line1, -40);
        line1.GetChild(0).Find("PosY").GetComponentInChildren<InputField>().text = "25";
        PosYAssign(line1, 25);
        line1.GetChild(0).Find("PosZ").GetComponentInChildren<InputField>().text = "400";
        PosZAssign(line1, 400);
        // draw track by default
        line1.GetChild(0).Find("CheckDraw").GetComponent<Toggle>().isOn = true;
        setObjTracing(line1, true);
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (ifStart)
        {
            if (Time.time - time_temp >= chart_time_step) // 画图的时间间隔
            {
                time_temp = Time.time;
                float theTime = Mathf.Round((time_temp - start_time)*100)/100.0f; // 数据保留1位小数
                //float theTime = time_temp - start_time;
                foreach (var v in nameObjectDict)
                {
                    Transform t = v.Value.gameObject.transform;
                    gameObjectData theDate = new gameObjectData(t.position, t.eulerAngles, t.localScale);
                    v.Value.dataDict.Add(theTime, theDate);
                    // draw points
                    v.Value.addTrace(t.position);

                }
                if (ifCheck)
                {
                    checkCount++;
                    // 检查25个数据
                    if (checkCount == 25)
                    {
                        checkPanel.GetComponent<CheckController>().onCheckFinish();
                        endButtonOnClick();
                        checkCount = 0; // clear
                    }
                }
            }
        }
    }

    //public void test1ButtonOnClick()
    //{
    //    if (!ifStart)
    //    {
    //        if (t1a)
    //        {
    //            t1a = false;
    //            theTest1.gameObject.SetActive(false);
    //        }
    //        else
    //        {
    //            t1a = true;
    //            theTest1.gameObject.SetActive(true);
    //        }
    //    }
    //}

    //public void test2ButtonOnClick()
    //{
    //    if (!ifStart)
    //    {
    //        if (t2a)
    //        {
    //            t2a = false;
    //            theTest2.gameObject.SetActive(false);
    //        }
    //        else
    //        {
    //            t2a = true;
    //            theTest2.gameObject.SetActive(true);
    //        }
    //    }
    //}

    //public void test3ButtonOnClick()
    //{
    //    if (!ifStart)
    //    {
    //        if (t3a)
    //        {
    //            t3a = false;
    //            theTest3.gameObject.SetActive(false);
    //        }
    //        else
    //        {
    //            t3a = true;
    //            theTest3.gameObject.SetActive(true);
    //        }
    //    }
    //}

    // 在报错的窗口点击确认
    public void exceptionConfirmButton()
    {
        exceptionField.SetActive(false);
    }

    public int setCodeName(Transform cf, string name, CodeType type, string importcode)
    {
        try
        {
            if (name == null || name == "")
            {
                throw new ExceptionWithOutLine("脚本名称不能为空");
            }
            string name_temp = codeDict[cf].codeName;
            codeDict[cf].codeName = name;
            if (name_temp != null)
            {
                nameCodeDict.Remove(name_temp);
            }
            if (nameCodeDict.ContainsKey(name))
            {
                codeDict[cf].codeName = null;
                throw new ExceptionWithOutLine("脚本名已存在，请重新命名");
            }
            else
            {
                nameCodeDict.Add(name, codeDict[cf]);
                // 设置代码框架
                setCodeFrame(cf, name, type, importcode);

            }
            //成功新建代码区域后，对选择模板和输入名字的编辑框做些调整，包括名字的添加，模板块不再显示，宽度调整
            cf.transform.GetChild(3).GetChild(0).GetComponent<InputField>().text = name;
            cf.transform.GetChild(3).GetChild(1).gameObject.SetActive(false);
            cf.transform.GetChild(3).GetComponent<RectTransform>().sizeDelta = new Vector2(300, 130);

            //截断过长的名字，用来在按钮上做显示
            if (name.Length > 6)
            {
                name = name.Substring(0, 6) + "...";
            }
            cf.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = name;
            return 0;
        }
        catch (ExceptionWithOutLine e)
        {
            exceptionField.SetActive(true);
            exceptionText.text = e.getInformation();
            return -1;
        }
    }

    // 给新建的空文件添加指定的代码框架
    private void setCodeFrame(Transform cf, string name, CodeType type, string importcode)
    {
        if (codeDict[cf].textForCode != null && codeDict[cf].textForCode != "")
        {
            return;
        }
        string code;
        if (importcode != "") code = importcode;
        else
        {
            switch (type)
            {
                case CodeType.BOUNCE:
                    code = BounceFrame.getCodeFrame(name);
                    break;
                case CodeType.BOUNCE_SOLUTION:
                    code = BounceCode.getCodeFrame(name);
                    break;
                case CodeType.BOUNCE1_SOLUTION:
                    code = Bounce1Code.getCodeFrame(name);
                    break;
                case CodeType.THROW:
                    code = ThrowFrame.getCodeFrame(name);
                    break;
                case CodeType.THROW_SOLUTION:
                    code = ThrowCode.getCodeFrame(name);
                    break;
                case CodeType.THROW1_SOLUTION:
                    code = Throw1Code.getCodeFrame(name);
                    break;
                case CodeType.PENDULUM:
                    code = PendulumFrame.getCodeFrame(name);
                    break;
                case CodeType.PENDULUM_SOLUTION:
                    code = PendulumCode.getCodeFrame(name);
                    break;
                case CodeType.PENDULUM1_SOLUTION:
                    code = Pendulum1Code.getCodeFrame(name);
                    break;
                default:
                    code = CodeFrame.getCodeFrame(name);
                    break;
            }
        }
        cf.transform.GetChild(1).GetComponent<InputField>().text = code;
        codeDict[cf].textForCode = code;
    }

    // 选中给定的文件，填充text，其对应的tab button上移
    public void codeFieldUp(Transform cf)
    {
        cf.transform.SetAsLastSibling();
        cf.transform.GetChild(1).GetComponent<InputField>().text = codeDict[cf].textForCode;
        cf.transform.position += new Vector3(0, +delta_y, 0);
        currentField = cf;
    }

    public string getCode(Transform cf)
    {
        return codeDict[cf].textForCode;
    }

    public void updateCode(string code)
    {
        nameCodeDict[codeOnEdit].textForCode = code;
        codeDict[currentField].textForCode = code;
        currentField.transform.GetChild(1).GetComponent<InputField>().text = code;
    }
    // 更新code，输入code时重新对textForCode赋值
    public void CodeTextAssign(Transform cf)
    {
        try
        {
            codeDict[cf].textForCode = cf.transform.GetChild(1).GetComponent<InputField>().text;
            //codeDict[cf].textForCode = cf.transform.GetChild(1).GetChild(0).GetChild(0).GetComponentInChildren<InputField>().text;
        }
        catch (ExceptionWithOutLine e)
        {
            exceptionField.SetActive(true);
            exceptionText.text = codeDict[cf].codeName + "  " + e.getInformation();
        }
    }

    // 取消当前选中的文件，将其对应的tab button下移
    public void codeFieldDown()
    {
        if (currentField == null)
        {
            return;
        }
        codeDict[currentField].textForCode = currentField.transform.GetChild(1).GetComponent<InputField>().text;
        currentField.transform.GetChild(2).gameObject.SetActive(false);
        currentField.transform.GetChild(3).gameObject.SetActive(false);
        currentField.transform.position += new Vector3(0, -delta_y, 0); // button位置向下，从选中变为没有选中
    }

    // 添加球体
    public Transform AddSphereButtonObject()
    {
        GameObject new_object_field = Instantiate(sphereObjectFieldPrefab, objectPartContent.transform) as GameObject;
        GameObject sphereGameObject = Instantiate(spherePrefab, gameObject.transform) as GameObject;
        HaHaObject haHaObject = new HaHaObject(null, sphereGameObject);
        haHaObject.type = "sphere";
        new_object_field.GetComponent<objectFieldController>().setData(haHaObject.init_pos, haHaObject.init_rot, haHaObject.init_scale);
        objectDict.Add(new_object_field.transform, haHaObject);
        addButton_object.transform.SetAsLastSibling();
        return new_object_field.transform;
    }

    public void AddSphereButtonObjectOnClick()
    {
        AddSphereButtonObject();
    }

    // 添加立方体
    public Transform AddCubeButtonObject()
    {
        GameObject new_object_field = Instantiate(cubeObjectFieldPrefab, objectPartContent.transform) as GameObject;
        GameObject cubeGameObject = Instantiate(cubePrefab, gameObject.transform) as GameObject;
        HaHaObject haHaObject = new HaHaObject(null, cubeGameObject);
        haHaObject.type = "cube";
        haHaObject.material = 1;
        new_object_field.GetComponent<objectFieldController>().setData(haHaObject.init_pos, haHaObject.init_rot, haHaObject.init_scale);
        objectDict.Add(new_object_field.transform, haHaObject);
        addButton_object.transform.SetAsLastSibling();
        return new_object_field.transform;
    }

    public void AddCubeButtonObjectOnClick()
    {
        AddCubeButtonObject();
    }
    public void objectNameAssign(Transform of, string name)
    {
        try
        {
            //if (name.Length == 0)
            //{
            //    return;
            //}
            string name_temp = objectDict[of].objectName;
            objectDict[of].objectName = name;
            if (name_temp != null)
            {
                nameObjectDict.Remove(name_temp);
            }
            if (nameObjectDict.ContainsKey(name))
            {
                objectDict[of].objectName = null;
                throw new ExceptionWithOutLine("对象名已存在，请重新命名");
            }
            else
            {
                nameObjectDict[name] = objectDict[of];
                nameObjectDict[name].gameObject.name = name;
            }
            if (name_temp == null)
            {
                dropdown.options.Add(new Dropdown.OptionData(name));
                if (dropdown.options.Count == 1) {
                    dropdown.transform.GetChild(0).GetComponent<Text>().text = name;
                    checkPanel.GetComponent<CheckController>().ChooseObject(0);
                }
            }
            else
            {
                for (int i = 0; i < dropdown.options.Count; ++i)
                {
                    if (dropdown.options[i].text == name_temp)
                    {
                        dropdown.options[i].text = name;
                        break;
                    }
                }
            }
        }
        catch (ExceptionWithOutLine e)
        {
            exceptionField.SetActive(true);
            exceptionText.text = e.getInformation();
        }
    }
    
    // 检查是否存在指定名字的对象
    public bool checkObjectName(string name)
    {
        try
        {
            if (name == null || name == "")
            {
                throw new ExceptionWithOutLine("请选择一个对象");
            }
            if (!nameObjectDict.ContainsKey(name))
            {
                throw new ExceptionWithOutLine("对象 " + name + " 不存在");
            }
            else
            {
                return true;
            }
        }
        catch (ExceptionWithOutLine e)
        {
            exceptionField.SetActive(true);
            exceptionText.text = e.getInformation();
            return false;
        }
    }

    // 更新对象绑定的代码
    public void codeNameAssign(Transform of, string codeName)
    {
        try
        {
            if (codeName == null)
            {
                objectDict[of].code_exe = null;
                return;
            }
            if (!nameCodeDict.ContainsKey(codeName))
            {
                throw new ExceptionWithOutLine("脚本不存在");
            }
        }
        catch (ExceptionWithOutLine e)
        {
            exceptionField.SetActive(true);
            exceptionText.text = e.getInformation();
        }
    }

    public void PosXAssign(Transform of, float num)
    {
        objectDict[of].init_pos = new Vector3(num, objectDict[of].init_pos.y, objectDict[of].init_pos.z);
        objectDict[of].update_gameObject_pos();
    }

    public void PosYAssign(Transform of, float num)
    {
        objectDict[of].init_pos = new Vector3(objectDict[of].init_pos.x, num, objectDict[of].init_pos.z);
        objectDict[of].update_gameObject_pos();
    }

    public void PosZAssign(Transform of, float num)
    {
        objectDict[of].init_pos = new Vector3(objectDict[of].init_pos.x, objectDict[of].init_pos.y, num);
        objectDict[of].update_gameObject_pos();
    }

    public void RotXAssign(Transform of, float num)
    {
        objectDict[of].init_rot = new Vector3(num, objectDict[of].init_rot.y, objectDict[of].init_rot.z);
        objectDict[of].update_gameObject_rot();
    }

    public void RotYAssign(Transform of, float num)
    {
        objectDict[of].init_rot = new Vector3(objectDict[of].init_rot.x, num, objectDict[of].init_rot.z);
        objectDict[of].update_gameObject_rot();
    }

    public void RotZAssign(Transform of, float num)
    {
        objectDict[of].init_rot = new Vector3(objectDict[of].init_rot.x, objectDict[of].init_rot.y, num);
        objectDict[of].update_gameObject_rot();
    }

    public void ScaleXAssign(Transform of, float num)
    {
        objectDict[of].init_scale = new Vector3(num, objectDict[of].init_scale.y, objectDict[of].init_scale.z);
        objectDict[of].update_gameObject_scale();
    }

    public void ScaleYAssign(Transform of, float num)
    {
        objectDict[of].init_scale = new Vector3(objectDict[of].init_scale.x, num, objectDict[of].init_scale.z);
        objectDict[of].update_gameObject_scale();
    }

    public void ScaleZAssign(Transform of, float num)
    {
        objectDict[of].init_scale = new Vector3(objectDict[of].init_scale.x, objectDict[of].init_scale.y, num);
        objectDict[of].update_gameObject_scale();
    }

    public void changeMat(Transform of, Material mat, int material)
    {
        objectDict[of].gameObject.GetComponent<MeshRenderer>().material = mat;
        objectDict[of].material = material;
    }

    // 删除代码文件，调整tab宽度和"+"按钮位置
    public void deleteButtonOnClick(Transform cf)
    {
        // 删除后其他tab的大小需要调整,修改current_width
        if (field_num >= 5)
        {
            current_width = current_width * (field_num / (field_num - 1));
            int temp = 1;
            bool bool_temp = false;
            foreach (var v in codeDict)
            {
                if (v.Key != cf)
                {
                    v.Key.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(current_width, v.Key.GetChild(0).GetComponent<RectTransform>().sizeDelta.y);
                    v.Key.GetChild(0).position = new Vector3((2 * temp - 1) * current_width / 2, v.Key.GetChild(0).position.y, v.Key.GetChild(0).position.z);
                    temp++;
                    if (bool_temp && currentField == cf)
                    {
                        codeFieldUp(v.Key);
                    }
                }
                else
                {
                    bool_temp = true;
                }

                if (temp == codeDict.Count && !bool_temp)
                {
                    codeFieldUp(v.Key);
                }
            }
        }
        // 不需要调整current_width
        else
        {
            int temp = 1;
            bool bool_temp = false;
            foreach (var v in codeDict)
            {
                if (v.Key != cf)
                {
                    v.Key.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(current_width, v.Key.GetChild(0).GetComponent<RectTransform>().sizeDelta.y);
                    v.Key.GetChild(0).position = new Vector3((2 * temp - 1) * current_width / 2, v.Key.GetChild(0).position.y, v.Key.GetChild(0).position.z);
                    temp++;
                    if (bool_temp && currentField == cf)
                    {
                        codeFieldUp(v.Key);
                    }
                }
                else
                {
                    bool_temp = true;
                }

                if (temp == codeDict.Count && !bool_temp)
                {
                    codeFieldUp(v.Key);
                }
            }
        }
        string name_temp = codeDict[cf].codeName;
        if (name_temp != null)
        {
            nameCodeDict.Remove(name_temp);
        }
        codeDict.Remove(cf);
        // 删除前至多有4个的时候，删除一个后"+"按钮需要移动
        if (field_num <= 4)
        {
            addButton_code.transform.position -= new Vector3(init_width, 0, 0);
        }
        field_num--;
        Destroy(cf.gameObject);
    }

    public void endButtonOnClick()
    {
        foreach (var v in nameObjectDict)
        {
            v.Value.gameObject_init();
            if (v.Value.code_exe != null)
            {
                v.Value.gameObject.GetComponent<gameObjectController>().EndFlag();
            }
            if (v.Value.chartField == null)
            {
                GameObject new_chart_field = Instantiate(chartField, chartPartContent.transform) as GameObject;
                new_chart_field.GetComponent<chartFieldController>().setLineChart(v.Value.dataDict, v.Key);
                v.Value.chartField = new_chart_field.transform;
            }
            else
            {
                v.Value.chartField.GetComponent<chartFieldController>().setLineChart(v.Value.dataDict, v.Key);
            }
            v.Value.dataDict.Clear();
        }
        if (!ifCheck)
        {
            startButton.GetComponent<Button>().interactable = true;
            endButton.GetComponent<Button>().interactable = false;
            checkButton.GetComponent<Button>().interactable = true;
        }
        //if (t1a)
        //{
        //    theTest1.end();
        //}
        //if (t2a)
        //{
        //    theTest2.end();
        //}
        //if (t3a)
        //{
        //    theTest3.end();
        //}
        ifStart = false;
    }
    public void startOnClick(string code)
    {
        nameCodeDict[codeOnEdit].textForCode = code;
        startButtonOnClick();
    }
    public void startButtonOnClick()
    {
        try
        {
            if (!ifCheck)
            {
                startButton.GetComponent<Button>().interactable = false;
                endButton.GetComponent<Button>().interactable = true;
                checkButton.GetComponent<Button>().interactable = false ;
            }
            foreach (var v in nameCodeDict)
            {
                check_code_name = v.Value.codeName;
                v.Value.setNode(nameObjectDict);
            }
            foreach (var v in objectDict)
            {
                if (v.Value.objectName == null)
                {
                    throw new ExceptionWithOutLine("对象未命名或重复命名");
                }
                string codeName = v.Key.GetChild(0).GetChild(2).GetChild(0).GetComponent<InputField>().text;
                if (codeName == null)
                {
                    continue;
                }
                if(codeName.Length == 0)
                {
                    continue;
                }
                if (!nameCodeDict.ContainsKey(codeName))
                {
                    throw new ExceptionWithOutLine("文件:" + codeName + " 不存在");
                }
                else
                {
                    Code_exe the_code_exe = new Code_exe(codeName);
                    v.Value.code_exe = the_code_exe;
                    the_code_exe.exe_init(v.Value, nameObjectDict, nameCodeDict[codeName].getNode());
                }
            }
            foreach (var v in objectDict)
            {
                if (v.Value.code_exe != null)
                {
                    //Debug.Log(v.Value.objectName);
                    // 清空上一次运行绘制的运动轨迹
                    v.Value.clearTrace();
                    // 画出初始位置点
                    v.Value.addTrace(v.Value.init_pos);
                    // 开始执行
                    v.Value.gameObject.GetComponent<gameObjectController>().StartFlag(v.Value.code_exe);
                }
            }
            //if (t1a)
            //{
            //    theTest1.start();
            //}
            //if (t2a)
            //{
            //    theTest2.start();
            //}
            //if (t3a)
            //{
            //    theTest3.start();
            //}
            ifStart = true;
            time_temp = Time.time;
            start_time = Time.time;
        }
        catch (ExceptionWithOutLine e)
        {
            if (!ifCheck)
            {
                startButton.GetComponent<Button>().interactable = true;
                endButton.GetComponent<Button>().interactable = false;
                checkButton.GetComponent<Button>().interactable = true;
            }
            exceptionField.SetActive(true);
            exceptionText.text = e.getInformation();
            //if (t1a)
            //{
            //    theTest1.end();
            //}
            //if (t2a)
            //{
            //    theTest2.end();
            //}
            //if (t3a)
            //{
            //    theTest3.end();
            //}
            ifStart = false;
            if (ifCheck)
            {
                checkPanel.GetComponent<CheckController>().onCheckClick();
            }
        }
        catch (MyException e)
        {
            if (!ifCheck)
            {
                startButton.GetComponent<Button>().interactable = true;
                endButton.GetComponent<Button>().interactable = false;
                checkButton.GetComponent<Button>().interactable = true;
            }
            exceptionField.SetActive(true);
            exceptionText.text = "at " + check_code_name + ".cs:\n" + e.getInformation();
            //if (t1a)
            //{
            //    theTest1.end();
            //}
            //if (t2a)
            //{
            //    theTest2.end();
            //}
            //if (t3a)
            //{
            //    theTest3.end();
            //}
            ifStart = false;
            if (ifCheck)
            {
                checkPanel.GetComponent<CheckController>().onCheckClick();
            }
        }
    }

    public void AddButtonOnClick()
    {
        if (field_num >= 7)
        {
            exceptionField.SetActive(true);
            exceptionText.text = new ExceptionWithOutLine("以达到代码区域的最大限制行数").getInformation();
            return;
        }
        if (field_num >= 4)
        {
            current_width = current_width * (field_num / (field_num + 1));
            int temp = 1;
            foreach (var v in codeDict)
            {
                v.Key.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(current_width, v.Key.GetChild(0).GetComponent<RectTransform>().sizeDelta.y);
                v.Key.GetChild(0).position = new Vector3((2 * temp - 1) * (current_width) / 2, v.Key.GetChild(0).position.y, v.Key.GetChild(0).position.z);
                temp++;
            }
        }
        GameObject new_code_field = Instantiate(codeFieldPrefab, codePart.transform) as GameObject;
        new_code_field.transform.GetChild(0).position += new Vector3(field_num * current_width - (init_width - current_width) / 2, 0, 0);
        new_code_field.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(current_width, new_code_field.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.y);
        new_code_field.transform.GetChild(3).gameObject.SetActive(true); // 和AddButtonOnClickForInit不一样的地方
        if (field_num <= 3)
        {
            addButton_code.transform.position += new Vector3(init_width, 0, 0);
        }
        field_num++;
        codeDict.Add(new_code_field.transform, new Code(null, null));
        codeFieldDown();
        codeFieldUp(new_code_field.transform);
    }

    // 创建一个新的代码编写区域
    // 更新已有的obj顶部button的位置和宽度
    // 通过prefab创建一个新的obj
    // 新obj的transform对象和code的键值对增加到字典中
    public Transform AddButtonOnClickForInit()
    {
        if (field_num >= 7) // 最多可创建7个脚本
        {
            return null;
        }
        if (field_num >= 4) // 大于等于三个脚本时需要更新tab的宽度和位置
        {
            current_width = current_width * (field_num / (field_num + 1));
            int temp = 1;
            //更新已有的代码顶部tag位置和宽度
            foreach (var v in codeDict)
            {
                // child0 是顶部显示文件名的按钮
                v.Key.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(current_width, v.Key.GetChild(0).GetComponent<RectTransform>().sizeDelta.y);
                v.Key.GetChild(0).position = new Vector3((2 * temp - 1) * (current_width) / 2, v.Key.GetChild(0).position.y, v.Key.GetChild(0).position.z);
                temp++;
            }
        }
        GameObject new_code_field = Instantiate(codeFieldPrefab, codePart.transform) as GameObject;
        new_code_field.transform.GetChild(0).position += new Vector3(field_num * current_width - (init_width - current_width) / 2, 0, 0);
        new_code_field.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(current_width, new_code_field.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.y);
        if (field_num <= 3)
        {
            addButton_code.transform.position += new Vector3(init_width, 0, 0);
        }
        field_num++;
        codeDict.Add(new_code_field.transform, new Code(null, null));
        codeFieldDown();
        codeFieldUp(new_code_field.transform);
        return new_code_field.transform;
    }

    public void exceptionDeliver(string e)
    {
        if (!ifCheck)
        {
            startButton.GetComponent<Button>().interactable = true;
            endButton.GetComponent<Button>().interactable = false;
            checkButton.GetComponent<Button>().interactable = true;
        }
        exceptionField.SetActive(true);
        exceptionText.text = e;
        //if (t1a)
        //{
        //    theTest1.end();
        //}
        //if (t2a)
        //{
        //    theTest2.end();
        //}
        //if (t3a)
        //{
        //    theTest3.end();
        //}
        ifStart = false;
        foreach (var v in nameObjectDict)
        {
            if (v.Value.code_exe != null)
            {
                v.Value.gameObject.GetComponent<gameObjectController>().EndFlag();
            }
            v.Value.gameObject_init();
            v.Value.dataDict.Clear();
        }
        if (ifCheck)
        {
            checkPanel.GetComponent<CheckController>().onCheckClick();
        }
    }

    public void onDeleteClick(Transform t)
    {
        HaHaObject theObj = objectDict[t];
        string name = theObj.objectName;
        if (theObj.gameObject != null)
        {
            Destroy(theObj.gameObject);
        }
        if (theObj.chartField != null)
        {
            Destroy(theObj.chartField.gameObject);
        }
        objectDict.Remove(t);
        if (name != null)
        {
            nameObjectDict.Remove(name);
            for(int i = 0; i < dropdown.options.Count; ++i)
            {
                if(dropdown.options[i].text == name)
                {
                    dropdown.options.Remove(dropdown.options[i]);
                    if (dropdown.transform.GetChild(0).GetComponent<Text>().text == name)
                    {
                        if (dropdown.options.Count != 0)
                        {
                            dropdown.transform.GetChild(0).GetComponent<Text>().text = dropdown.options[dropdown.value].text;
                            checkPanel.GetComponent<CheckController>().ChooseObject(dropdown.value);
                        }
                        else
                        {
                            dropdown.transform.GetChild(0).GetComponent<Text>().text = "";
                            checkPanel.GetComponent<CheckController>().ChooseObject(-1);
                        }
                    }
                    break;
                }               
            }
        }
        Destroy(t.gameObject);
    }

    public bool SaveFile(string path,string name, string text)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (text == "" || text == null)
        {
            exceptionField.SetActive(true);
            exceptionText.text = "Empty file";
            return false;
        }
        ClickSaveScript(name, text);
        exceptionField.SetActive(true);
        exceptionText.text = "Save successfully";
        return true;
#endif
#if UNITY_EDITOR
        if (path == "" || path == null)
        {
            exceptionField.SetActive(true);
            exceptionText.text = "路径不能为空";
            return false;
        }
        if (name == "" || name == null)
        {
            exceptionField.SetActive(true);
            exceptionText.text = "文件名不能为空";
            return false;
        }
        if (text == "" || text == null)
        {
            exceptionField.SetActive(true);
            exceptionText.text = "文件为空";
            return false;
        }
        if (!Directory.Exists(path))
        {
            exceptionField.SetActive(true);
            exceptionText.text = "文件路径不存在";
            return false;
        }
        string savePath = path + "/" + name;
        if (File.Exists(savePath))
        {
            exceptionField.SetActive(true);
            exceptionText.text = "文件已存在";
            return false;
        }
        byte[] byteArray = System.Text.Encoding.Default.GetBytes(text);
        File.WriteAllBytes(savePath, byteArray);
        exceptionField.SetActive(true);
        exceptionText.text = "保存成功";
        return true;
#endif
    }

    public string getCodeName(Transform t)
    {
        Code code = codeDict[t];
        return code.codeName;
    }

    // 更新是否要绘制对象的运动轨迹
    public void setObjTracing(Transform t, bool value)
    {
        objectDict[t].tracing = value;
    }

    // 设置是否是评分状态
    public void setIfCheck(bool value)
    {
        ifCheck = value;
    }

    // 设置开始结束按钮的可交互性
    public void updateStartEndBtn(bool start, bool end)
    {
        startButton.GetComponent<Button>().interactable = start;
        endButton.GetComponent<Button>().interactable = end;
    }

    // 获取某个对象的运动数据
    public Dictionary<float, gameObjectData> getData(string name)
    {
        return nameObjectDict[name].dataDict;
    }

    //导入了不是cs格式的文件
    public void NotCSimport()
    {
        exceptionField.SetActive(true);
        exceptionText.text = "请选择 .cs文件";
    }

    public void DealwithImport(string data)
    {
        int i = data.IndexOf('~');
        string name = data.Substring(0, i);
        string code = data.Substring(i + 1);
        currentField.GetChild(3).GetChild(0).GetComponent<InputField>().text = name;
        currentField.gameObject.SendMessage("SetImportcode", code);
    }

    public void SetException(string text)
    {
        exceptionField.SetActive(true);
        exceptionText.text = text;
    }

    public void ReportComplete()
    {
        ReportWait.SetActive(false);
    }

    public void quitSave()
    {
        isQuitUI.SetActive(false);
        Save(true);
        //GM.GM_Core.instance.toBackScene(); // back when saving successfully
    }

    public void quitUnsave()
    {
        isQuitUI.SetActive(false);
        GM.GM_Core.instance.toBackScene();
    }
    public void quitCancel()
    {
        isQuitUI.SetActive(false);
    }
    public void Save(bool isQuiting)
    {
        savedScene.objectInfos = new List<GameManager.ObjectInfo>();
        savedScene.codeInfos = new List<GameManager.CodeInfo>();
        foreach (var v in objectDict)
        {
            HaHaObject theObject = v.Value;
            Transform theField = v.Key;
            string type = theObject.type;
            string name = theField.GetChild(0).Find("name").GetComponentInChildren<InputField>().text;
            string className = theField.GetChild(0).Find("class name").GetComponentInChildren<InputField>().text;
            Vector3 pos = theObject.init_pos;
            Vector3 rot = theObject.init_rot;
            Vector3 scale = theObject.init_scale;
            int material = theObject.material;
            bool tracing = theObject.tracing;

            GameManager.ObjectInfo objectInfo = new GameManager.ObjectInfo();
            objectInfo.type = type;
            objectInfo.name = name;
            objectInfo.className = className;
            objectInfo.pos = pos;
            objectInfo.rot = rot;
            objectInfo.scale = scale;
            objectInfo.material = material;
            objectInfo.tracing = tracing;

            savedScene.objectInfos.Add(objectInfo);
        }
        foreach (var v in codeDict)
        {
            
            Code theCode = v.Value;
            string codeName = theCode.codeName;
            string codeText = theCode.textForCode;

            GameManager.CodeInfo codeInfo = new GameManager.CodeInfo();
            codeInfo.codeName = codeName;
            codeInfo.codeText = codeText;

            savedScene.codeInfos.Add(codeInfo);

        }
        //GM.GM_Core.instance.sceneString = JsonUtility.ToJson(savedScene);
        string sceneString = JsonUtility.ToJson(savedScene);
        GM.GM_Core.instance.SaveScene(sceneString, isQuiting);
    }
    public void Restore(string sceneJson)
    {
        savedScene = JsonUtility.FromJson<Chemix.GameManager.ExperimentalScene>(GM.GM_Core.instance.sceneString);
        foreach (var v in savedScene.codeInfos)
        {
            Transform code_transform = AddButtonOnClickForInit();
            setCodeName(code_transform, v.codeName, CodeType.BOUNCE_SOLUTION, v.codeText);
        }

        foreach (var v in savedScene.objectInfos)
        {
            switch(v.type)
            {
                case "cube":
                    {
                        Transform cube = AddCubeButtonObject();
                        cube.GetChild(0).Find("name").GetComponentInChildren<InputField>().text = v.name;
                        objectNameAssign(cube, v.name);
                        cube.GetChild(0).Find("class name").GetComponentInChildren<InputField>().text = v.className;
                        codeNameAssign(cube, v.className);
                        setObjTracing(cube, v.tracing);
                        if (v.material != 1)//default material
                        {
                            switch (v.material)
                            {
                                case 2:
                                    {
                                        Material mat = cube.GetComponent<objectFieldController>().mat2;
                                        cube.GetChild(0).Find("Mat2").GetComponent<Toggle>().isOn = true;
                                        changeMat(cube, mat, 2);
                                        Sprite img = cube.GetComponent<objectFieldController>().img2;
                                        cube.GetComponent<objectFieldController>().image.sprite = img;
                                        break;
                                    }
                                case 3:
                                    {
                                        Material mat = cube.GetComponent<objectFieldController>().mat3;
                                        cube.GetChild(0).Find("Mat3").GetComponent<Toggle>().isOn = true;
                                        changeMat(cube, mat, 3);
                                        Sprite img = cube.GetComponent<objectFieldController>().img3;
                                        cube.GetComponent<objectFieldController>().image.sprite = img;
                                        break;
                                    }
                            }
                        }
                        Vector3 pos = v.pos;
                        Vector3 rot = v.rot;
                        Vector3 scale = v.scale;
                        bool isTracing = v.tracing;
                        cube.GetChild(0).Find("PosX").GetComponentInChildren<InputField>().text = v.pos.x.ToString();
                        cube.GetChild(0).Find("PosY").GetComponentInChildren<InputField>().text = v.pos.y.ToString();
                        cube.GetChild(0).Find("PosZ").GetComponentInChildren<InputField>().text = v.pos.z.ToString();
                        cube.GetChild(0).Find("RotX").GetComponentInChildren<InputField>().text = v.rot.x.ToString();
                        cube.GetChild(0).Find("RotY").GetComponentInChildren<InputField>().text = v.rot.y.ToString();
                        cube.GetChild(0).Find("RotZ").GetComponentInChildren<InputField>().text = v.rot.z.ToString();
                        cube.GetChild(0).Find("ScaX").GetComponentInChildren<InputField>().text = v.scale.x.ToString();
                        cube.GetChild(0).Find("SczY").GetComponentInChildren<InputField>().text = v.scale.y.ToString();
                        cube.GetChild(0).Find("SczZ").GetComponentInChildren<InputField>().text = v.scale.z.ToString();
                        cube.GetChild(0).Find("CheckDraw").GetComponent<Toggle>().isOn = isTracing;
                        PosXAssign(cube, pos.x);
                        PosYAssign(cube, pos.y);
                        PosZAssign(cube, pos.z);
                        RotXAssign(cube, rot.x);
                        RotYAssign(cube, rot.y);
                        RotZAssign(cube, rot.z);
                        ScaleXAssign(cube, scale.x);
                        ScaleYAssign(cube, scale.y);
                        ScaleZAssign(cube, scale.z);
                        break;
                    }

                case "sphere":
                    {
                        Transform sphere = AddSphereButtonObject();
                        sphere.GetChild(0).Find("name").GetComponentInChildren<InputField>().text = v.name;
                        objectNameAssign(sphere, v.name);
                        sphere.GetChild(0).Find("class name").GetComponentInChildren<InputField>().text = v.className;
                        codeNameAssign(sphere, v.className);
                        setObjTracing(sphere, v.tracing);
                        if (v.material != 1)//default material
                        {
                            switch (v.material)
                            {
                                case 2:
                                    {
                                        Material mat = sphere.GetComponent<objectFieldController>().mat2;
                                        sphere.GetChild(0).Find("Mat2").GetComponent<Toggle>().isOn = true;
                                        changeMat(sphere, mat, 2);
                                        Sprite img = sphere.GetComponent<objectFieldController>().img2;
                                        sphere.GetComponent<objectFieldController>().image.sprite = img;
                                        break;
                                    }
                                case 3:
                                    {
                                        Material mat = sphere.GetComponent<objectFieldController>().mat3;
                                        sphere.GetChild(0).Find("Mat3").GetComponent<Toggle>().isOn = true;
                                        changeMat(sphere, mat, 3);
                                        Sprite img = sphere.GetComponent<objectFieldController>().img3;
                                        sphere.GetComponent<objectFieldController>().image.sprite = img;
                                        break;
                                    }
                            }
                        }
                        Vector3 pos = v.pos;
                        Vector3 rot = v.rot;
                        Vector3 scale = v.scale;
                        bool isTracing = v.tracing;
                        sphere.GetChild(0).Find("PosX").GetComponentInChildren<InputField>().text = v.pos.x.ToString();
                        sphere.GetChild(0).Find("PosY").GetComponentInChildren<InputField>().text = v.pos.y.ToString();
                        sphere.GetChild(0).Find("PosZ").GetComponentInChildren<InputField>().text = v.pos.z.ToString();
                        sphere.GetChild(0).Find("RotX").GetComponentInChildren<InputField>().text = v.rot.x.ToString();
                        sphere.GetChild(0).Find("RotY").GetComponentInChildren<InputField>().text = v.rot.y.ToString();
                        sphere.GetChild(0).Find("RotZ").GetComponentInChildren<InputField>().text = v.rot.z.ToString();
                        sphere.GetChild(0).Find("ScaX").GetComponentInChildren<InputField>().text = v.scale.x.ToString();
                        sphere.GetChild(0).Find("SczY").GetComponentInChildren<InputField>().text = v.scale.y.ToString();
                        sphere.GetChild(0).Find("SczZ").GetComponentInChildren<InputField>().text = v.scale.z.ToString();
                        sphere.GetChild(0).Find("CheckDraw").GetComponent<Toggle>().isOn = isTracing;
                        PosXAssign(sphere, pos.x);
                        PosYAssign(sphere, pos.y);
                        PosZAssign(sphere, pos.z);
                        RotXAssign(sphere, rot.x);
                        RotYAssign(sphere, rot.y);
                        RotZAssign(sphere, rot.z);
                        ScaleXAssign(sphere, scale.x);
                        ScaleYAssign(sphere, scale.y);
                        ScaleZAssign(sphere, scale.z);
                        break;
                    }
            }
        }
    }

    public void loadExperiment()
    {
        //if (SceneManager.GetSceneByName("EditPlatformScene").isLoaded)
        //    SceneManager.UnloadSceneAsync("EditPlatformScene");
        //GM.GM_Core.instance.used = true;
        //SceneManager.LoadScene("EditPlatformScene");

        GM.GM_Core.instance.used = true;
        GM.GM_Core.instance.SwitchToScene("EditPlatformScene");
    }


    public void SetupExperimentObject(Transform Obj, string ObjName, string codeName, Vector3 pos, Vector3 sca, bool trace)
    {
        // set name
        Obj.GetChild(0).Find("name").GetComponentInChildren<InputField>().text = ObjName;
        objectNameAssign(Obj, ObjName);

        // set class name
        Obj.GetChild(0).Find("class name").GetComponentInChildren<InputField>().text = codeName;
        codeNameAssign(Obj, codeName);

        // set position
        Obj.GetChild(0).Find("PosX").GetComponentInChildren<InputField>().text = pos.x.ToString();
        PosXAssign(Obj, pos.x);
        Obj.GetChild(0).Find("PosY").GetComponentInChildren<InputField>().text = pos.y.ToString();
        PosYAssign(Obj, pos.y);
        Obj.GetChild(0).Find("PosZ").GetComponentInChildren<InputField>().text = pos.z.ToString();
        PosZAssign(Obj, pos.z);

        // set scale
        Obj.GetChild(0).Find("ScaX").GetComponentInChildren<InputField>().text = sca.x.ToString();
        ScaleXAssign(Obj, sca.x);
        Obj.GetChild(0).Find("SczY").GetComponentInChildren<InputField>().text = sca.y.ToString();
        ScaleYAssign(Obj, sca.y);
        Obj.GetChild(0).Find("SczZ").GetComponentInChildren<InputField>().text = sca.z.ToString();
        ScaleZAssign(Obj, sca.z);

        // draw track by default
        Obj.GetChild(0).Find("CheckDraw").GetComponent<Toggle>().isOn = trace;
        setObjTracing(Obj, trace);
    }
}
