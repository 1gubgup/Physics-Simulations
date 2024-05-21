using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionPartController : MonoBehaviour
{
    public GameObject retButton;
    public GameObject comeButton;
    private int RetOrCome = 0;
    private Vector3 position1;
    private Vector3 position2;
    private Vector3 currentV;
    // Start is called before the first frame update
    void Start()
    {
        position1 = transform.position - new Vector3(0, 500, 0);
        position2 = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (RetOrCome == -1)
        {
            // 根据当前屏幕高度进行更新
            position2.y = Screen.height;
            transform.position = Vector3.SmoothDamp(transform.position, position2, ref currentV, 0.5f);
            if ((position2 - transform.position).y < 0.01)
            {
                RetOrCome = 0;
            }
        }
        else if (RetOrCome == 1)
        {
            // 根据当前屏幕高度进行更新
            position1.y = Screen.height - 500;
            transform.position = Vector3.SmoothDamp(transform.position, position1, ref currentV, 0.5f);
            if ((transform.position - position1).y < 0.01)
            {
                RetOrCome = 0;
            }
        }
    }

    public void come()
    {
        RetOrCome = 1;
        retButton.SetActive(true);
        comeButton.SetActive(false);
        // 隐藏返回主菜单的按键
        //GM.GM_Core.instance.setReturnButton(false);
        //GM.GM_Core.instance.setPicturePanel(false);
    }

    public void ret()
    {
        RetOrCome = -1;
        retButton.SetActive(false);
        comeButton.SetActive(true);
        // 恢复返回主菜单的按键
        GM.GM_Core.instance.setReturnButton(true);
        GM.GM_Core.instance.setPicturePanel(true);
    }
}
