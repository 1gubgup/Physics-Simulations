using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
//using UnityEngine.Windows;

public class chartFieldController : MonoBehaviour
{
    public lineChartController posX;
    public lineChartController posY;
    public lineChartController posZ;
    public lineChartController rotX;
    public lineChartController rotY;
    public lineChartController rotZ;
    public lineChartController energyChart;
    //public lineChartController scaleX;
    //public lineChartController scaleY;
    //public lineChartController scaleZ;
    public Text chartName;

    private bool savePosX = false;
    private bool savePosY = false;
    private bool savePosZ = false;
    private bool saveRotX = false;
    private bool saveRotY = false;
    private bool saveRotZ = false;
    private bool saveEnergy = false;

    public GameObject exceptionField;
    public Text exceptionText;
    public GameObject SaveChartField;
    public InputField PathField, NameField;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void setLineChart(Dictionary<float, gameObjectData> dict, string n)
    {
        chartName.text = n;
        Dictionary<float, float> posXDict = new Dictionary<float, float>();
        Dictionary<float, float> posYDict = new Dictionary<float, float>();
        Dictionary<float, float> posZDict = new Dictionary<float, float>();
        Dictionary<float, float> rotXDict = new Dictionary<float, float>();
        Dictionary<float, float> rotYDict = new Dictionary<float, float>();
        Dictionary<float, float> rotZDict = new Dictionary<float, float>();
        Dictionary<float, float> energyDict = new Dictionary<float, float>();
        //Dictionary<float, float> scaleXDict = new Dictionary<float, float>();
        //Dictionary<float, float> scaleYDict = new Dictionary<float, float>();
        //Dictionary<float, float> scaleZDict = new Dictionary<float, float>();
        //Vector3 oldPos, newPos;
        //oldPos = Vector3.zero;
        foreach (var v in dict)
        {
 //           newPos = new Vector3(v.Value.pos.x, v.Value.pos.y, v.Value.pos.z);
 //           if (oldPos != Vector3.zero)
 //           {
 //               float energy = newPos.y * 9.8f
 //+ (newPos - oldPos).sqrMagnitude / 0.08f;
 //               energyDict.Add(v.Key, energy);
 //           }
 //           else
 //           {
 //               float energy = newPos.y * 9.8f; // 上一次位置为0时默认是第一次计算能量，简单认为初始状态动能为0
 //               energyDict.Add(v.Key, energy);
 //           }
            posXDict.Add(v.Key, v.Value.pos.x);
            posYDict.Add(v.Key, v.Value.pos.y);
            posZDict.Add(v.Key, v.Value.pos.z);
            rotXDict.Add(v.Key, v.Value.rot.x);
            rotYDict.Add(v.Key, v.Value.rot.y);
            rotZDict.Add(v.Key, v.Value.rot.z);
            //scaleXDict.Add(v.Key, v.Value.scale.x);
            //scaleYDict.Add(v.Key, v.Value.scale.y);
            //scaleZDict.Add(v.Key, v.Value.scale.z);
            //oldPos = newPos;
        }
        posX.transformData(posXDict);
        posY.transformData(posYDict);
        posZ.transformData(posZDict);
        rotX.transformData(rotXDict);
        rotY.transformData(rotYDict);
        rotZ.transformData(rotZDict);
        //energyChart.transformData(energyDict);
        //scaleX.transformData(scaleXDict);
        //scaleY.transformData(scaleYDict);
        //scaleZ.transformData(scaleZDict);
    }

    public void onPosXChanged(bool value)
    {
        savePosX = value;
    }

    public void onPosYChanged(bool value)
    {
        savePosY = value;
    }

    public void onPosZChanged(bool value)
    {
        savePosZ = value;
    }

    public void onRotXChanged(bool value)
    {
        saveRotX = value;
    }

    public void onRotYChanged(bool value)
    {
        saveRotY = value;
    }

    public void onRotZChanged(bool value)
    {
        saveRotZ = value;
    }

    public void onEnergyChanged(bool value)
    {
        saveEnergy = value;
    }

    // 点击保存图表按钮，弹出保存界面或异常界面
    public void onSaveClick()
    {
        if (savePosX || savePosY || savePosZ || saveRotX || saveRotY || saveRotZ || saveEnergy)
        {
            NameField.text = chartName.text;
            if (exceptionField.activeSelf == false)
            {
                SaveChartField.SetActive(true);
            } 
        }
        else
        {
            exceptionField.SetActive(true);
            exceptionText.text = "Please choose at least one chart to save.";
        }
    }

    // 点击确认保存按钮，弹出异常界面或成功保存文件
    public void onSaveConfirmClick()
    {
        string path = PathField.text;
        string name = NameField.text;
        // 路径为空
        if (path == "" || path == null)
        {
            sendException("Path shouldn't be empty");
            return;
        }
        // 文件名为空
        if (name == "" || name == null)
        {
            sendException("File name shouldn't be empty");
            return;
        }
        // 路径不存在
        if (!Directory.Exists(path))
        {
            sendException("File path doesn't exist");
            return;
        }
        string savePath = path + "/" + name +".csv";
        // 路径下有重名的文件
        if (File.Exists(savePath))
        {
            sendException("File already exists");
            return;
        }
        // 保存内容
        string content = getLineChartValues();
        byte[] byteArray = System.Text.Encoding.Default.GetBytes(content);
        File.WriteAllBytes(savePath, byteArray);
        sendException("Saved successfully.");
    }

    // 点击取消保存，隐藏保存界面
    public void onSaveCancelClick()
    {
        SaveChartField.SetActive(false);
    }

    // 获取图表数据，csv格式的字符串
    private string getLineChartValues()
    {
        // todo
        string content = "t";
        List<Pair> posXDict = new List<Pair>();
        List<Pair> posYDict = new List<Pair>();
        List<Pair> posZDict = new List<Pair>();
        List<Pair> rotXDict = new List<Pair>();
        List<Pair> rotYDict = new List<Pair>();
        List<Pair> rotZDict = new List<Pair>();
        int count = 0; // 至少要保存一个图表，多个图表时count是相同的
        if (savePosX)
        {
            posXDict = posX.getChartData();
            count = posXDict.Count;
            content += ",posX";
        }
        if (savePosY)
        {
            posYDict = posY.getChartData();
            count = posYDict.Count;
            content += ",posY";
        }
        if (savePosZ)
        {
            posZDict = posZ.getChartData();
            count = posZDict.Count;
            content += ",posZ";
        }
        if (saveRotX)
        {
            rotXDict = rotX.getChartData();
            count = rotXDict.Count;
            content += ",rotX";
        }
        if (saveRotY)
        {
            rotYDict = rotY.getChartData();
            count = rotYDict.Count;
            content += ",rotY";
        }
        if (saveRotZ)
        {
            rotZDict = rotZ.getChartData();
            count = rotZDict.Count;
            content += ",rotZ";
        }
        content += "\n";

        // 开始遍历图表中所有数据
        for(int i = 0; i < count; i++)
        {
            string key = "";
            string values = "";
            if (savePosX)
            {
                key = posXDict[i].key;
                values += "," + posXDict[i].value;
            }
            if (savePosY)
            {
                key = posYDict[i].key;
                values += "," + posYDict[i].value;
            }
            if (savePosZ)
            {
                key = posZDict[i].key;
                values += "," + posZDict[i].value;
            }
            if (saveRotX)
            {
                key = rotXDict[i].key;
                values += "," + rotXDict[i].value;
            }
            if (saveRotY)
            {
                key = rotYDict[i].key;
                values += "," + rotYDict[i].value;
            }
            if (saveRotZ)
            {
                key = rotZDict[i].key;
                values += "," + rotZDict[i].value;
            }
            content += key + values + "\n";
        }
        return content;
    }

    // 点击异常界面的确认按钮
    public void exceptionConfirmButton()
    {
        exceptionField.SetActive(false);
    }

    // 隐藏保存界面，显示异常的内容
    private void sendException(string exp)
    {
        SaveChartField.SetActive(false);
        exceptionField.SetActive(true);
        exceptionText.text = exp;
    }
}
