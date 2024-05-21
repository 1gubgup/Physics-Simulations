using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 控制知识点界面 两个实验共用
public class QuestionUI : MonoBehaviour
{
    public GameObject questionPanel;

    public void showQuestionOnClick()
    {
        questionPanel.SetActive(true);
        // 隐藏返回主菜单的按键
        GM.GM_Core.instance.setReturnButton(false);
        GM.GM_Core.instance.setPicturePanel(false);
    }

    public void returnOnClick()
    {
        questionPanel.SetActive(false);
        // 恢复返回主菜单的按键
        GM.GM_Core.instance.setReturnButton(true);
        GM.GM_Core.instance.setPicturePanel(true);
    }
}
