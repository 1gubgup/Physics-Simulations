using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckThrow : MonoBehaviour
{
    private CSVUtil yCSV;
    private CSVUtil xCSV;

    // Start is called before the first frame update
    void Start()
    {
        xCSV = transform.gameObject.AddComponent<CSVUtil>();
        StartCoroutine(xCSV.GetData("pendulumEulerX.csv"));
        yCSV = transform.gameObject.AddComponent<CSVUtil>();
        StartCoroutine(yCSV.GetData("pendulumEulerY.csv"));
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

        }
        Debug.Log(count + ":" + score);
        return score / count;
    }

    private bool check(float x, float y, float time)
    {
        if (yCSV.data.ContainsKey(time.ToString()) && cmpWithStandard(x, time, xCSV)
            && cmpWithStandard(y, time, yCSV))
        {

            return true;
        }
        return false;
    }

    private bool cmpWithStandard(float value, float time, CSVUtil csv)
    {
        float standard = csv.data[time.ToString()];
        if (Mathf.Abs(value - standard) <= 0.0001)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
