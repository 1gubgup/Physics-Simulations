using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class GuideUI : MonoBehaviour
{
    public Text textLabel;
    public TextAsset textFile;
    private int index;

    public int scene;
    public Transform player;
    private int count;

    List<string> textList = new List<string>();

    private void OnEnable()
    {
        GetTextFromFile(textFile);
        textLabel.text = textList[index];
        index++;
    }

    void Update()
    {
        float position = player.position.z;

        if (scene == 1)
        {
            if (position < 1000.0f)
            {
                count = 0;
            }
            else if (position < 2000.0f)
            {
                count = 1;
            }
            else if (position < 3500.0f)
            {
                count = 2;
            }
            else if (position < 6000.0f)
            {
                count = 3;
            }
            else
            {
                count = 4;
            }
        }
        else if (scene == 2)
        {
            if (position < 150.0f)
            {
                count = 0;
            }
            else if (position < 300.0f)
            {
                count = 1;
            }
            else
            {
                count = 2;
            }
        }

        if (count==index)
        {
            textLabel.text = textList[index];
            index++;
        }
    }

    void GetTextFromFile(TextAsset file)
    {
        textList.Clear();
        index = 0;

        var lineData = file.text.Split('\n');

        foreach (var line in lineData)
        {
            textList.Add(line);
        }
    }
}
