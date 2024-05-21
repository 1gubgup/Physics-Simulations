using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuideController : MonoBehaviour
{
    public GameObject guideFeild;
    public Text hintText;
    public Button backBtn;
    public Button nextBtn;
    List<string> hints = new List<string>();

    int currentHint = 0;
    int hintCount = 0;
    
    public void onHelpClick()
    {
        guideFeild.SetActive(true);
    }

    public void onCloseClick()
    {
        guideFeild.SetActive(false);
    }

    public void onNextClick()
    {
        // first hint 
        if (currentHint == 0)
        {
            backBtn.interactable = true;
        }

        currentHint++;
        if (currentHint < hintCount-1)
        {
            hintText.text = hints[currentHint];
        }
        else if (currentHint == hintCount-1) // show the last hint
        {
            hintText.text = hints[currentHint];
            nextBtn.GetComponentInChildren<Text>().text = "完成";
        }
        else // finish all guidances
        {
            currentHint--; // handle overflow
            guideFeild.SetActive(false);
        }
    }

    public void onBackClick()
    {
        if(currentHint == hintCount-1)
        {
            nextBtn.GetComponentInChildren<Text>().text = "下一步";
        }
        currentHint--;
        hintText.text = hints[currentHint];
        if (currentHint == 0)
        {
            backBtn.interactable = false; // disable back button
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHint = 0;
        addHint("1. 创建脚本文件\n" +
            "点击左侧“代码”按钮，展开编程区域；\n点击上方“+”，创建新脚本；\n选择模板，例如“自由落体”；\n" +
            "输入文件名，例如“Fall”；\n点击“确认”创建指定模板。");
        addHint("2. 编写代码\n" +
            "在代码区域根据模板中的注释和已有内容补全关键代码。\n" +
            "可以点击“说明”按钮查看平台支持的语法。");
        addHint("3. 创建对象\n" +
            "点击右侧“对象”按钮，展开对象管理区域；\n" +
            "点击“球体”或“立方体”创建对应对象。");
        addHint("4. 设置对象属性\n" +
            "在右侧的对象编辑区域设置对象属性；\n" +
            "为对象设置唯一的名字；\n" +
            "可以调整位置、旋转、放缩、材质属性；\n" +
            "选择是否绘制运动轨迹，如果选中，会在运行时绘制其轨迹。");
        addHint("5. 绑定脚本\n" +
            "在对象编辑区域的“class”输入框输入脚本的名字即可完成对象和该脚本的绑定，" +
            "例如给对象绑定“Bounce”脚本，则输入“Bounce”。");
        addHint("6. 运行\n" +
            "点击“开始”按钮，开始执行脚本，对象会受脚本控制进行运动；\n" +
            "点击“结束”按钮，停止执行脚本。");
        addHint("7. 查看图表\n" +
            "点击下方“图表”按钮，可以查看上一次运行的对象的运动数据图表。");
        addHint("8. 评分\n" +
            "点击上方“评分”按钮，可以对实验要求实现的算法正确性进行评估。\n" +
            "选择要评估的算法类型，例如“抛物”；\n" +
            "按提示设置算法参数；\n" +
            "输入绑定了该算法的对象名；\n" +
            "点击“确定”按钮，等待评估结果。");
        hintCount = hints.Count;
        hintText.text = hints[0];
        backBtn.interactable = false;
    }

    private void addHint(string hint)
    {
        hints.Add(hint);
    }
}
