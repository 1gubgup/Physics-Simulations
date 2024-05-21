using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckBase : MonoBehaviour
{
    private CSVUtil yCSV;
    private CSVUtil xCSV;

    private bool checkX = false;
    private bool checkY = false;
    //public string fileNameX, fileNameY;

    public void getCSVData(string fileNameX, string fileNameY)
    {
        if(fileNameX != null && fileNameX != "")
        {
            checkX = true;
            xCSV = transform.gameObject.AddComponent<CSVUtil>();
            StartCoroutine(xCSV.GetData(fileNameX));
        }
        if (fileNameY != null && fileNameY != "")
        {
            checkY = true;
            yCSV = transform.gameObject.AddComponent<CSVUtil>();
            StartCoroutine(yCSV.GetData(fileNameY));
        }
    }

    public float compare(Dictionary<float, gameObjectData> data)
    {
        float score = 0;
        float count = 0;

        foreach (var v in data)
        {
            count++;
            float time = TimeUtil.Round(v.Key, 4, 100);
            float x = v.Value.pos.x;
            float y = v.Value.pos.y;
            if (check(x, y, time) || check(x, y, time - 0.04f))
            {
                score++;
            }
            else
            {
                //Debug.Log(time + ": " + x + ", " + y);
                check(x, y, time);
                check(x, y, time - 0.04f);
            }

        }
        //Debug.Log(count + ":" + score);
        return score / count;
    }

    private bool check(float x, float y, float time)
    {
        time = Mathf.Round(time * 100) / 100;
        if(checkX && checkY)
        {
            if (yCSV.data.ContainsKey(time.ToString()) && cmpWithStandard(x, time, xCSV)
            && cmpWithStandard(y, time, yCSV))
            {

                return true;
            }
        }
        else if (checkX)
        {
            if (xCSV.data.ContainsKey(time.ToString()) && cmpWithStandard(x, time, xCSV))
            {

                return true;
            }
        }
        else if (checkY)
        {
            if (yCSV.data.ContainsKey(time.ToString()) && cmpWithStandard(y, time, yCSV))
            {

                return true;
            }
        }
        return false;
    }

    private bool cmpWithStandard(float value, float time, CSVUtil csv)
    {
        float standard = csv.data[time.ToString()];
        if (Mathf.Abs(value - standard) <= 0.001)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
