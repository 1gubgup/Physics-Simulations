using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionController : MonoBehaviour
{
    public List<Toggle> toggles;
    public int rightAns = 0;
    public GameObject feedBackRight;
    public GameObject feedBackWrong;

    private int currentAns = 0;
    // Start is called before the first frame update
    void Start()
    {
        // add listener for toggles
        int len = toggles.Count;
        for (int i = 0; i < len; i++)
        {
            int index = i;
            toggles[i].onValueChanged.AddListener(delegate (bool value)
            {
                onValueChanged(index, value);
            });
        }
        feedBackRight.SetActive(false);
        feedBackWrong.SetActive(false);
    }

    // 选择要检查的算法
    private void onValueChanged(int index, bool value)
    {
        if (value)
        {
            currentAns = index;
        }
        feedBackRight.SetActive(false);
        feedBackWrong.SetActive(false);
    }

    public void onConfirmClick()
    {
        if (currentAns == rightAns)
        {
            feedBackRight.SetActive(true);
        }
        else
        {
            feedBackWrong.SetActive(true);
        }
    }
}
