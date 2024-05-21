using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckBounce : MonoBehaviour
{

    private CSVUtil bounceCSV;

    // Start is called before the first frame update
    void Start()
    {
        bounceCSV = transform.gameObject.AddComponent<CSVUtil>();
        StartCoroutine(bounceCSV.GetData("bounceEuler.csv"));
    }

    public float compare(Dictionary<float, gameObjectData> data)
    {
        float score = 0;
        float count = 0;
        
        foreach (var v in data)
        {
            count++;
            float time = TimeUtil.Round(v.Key, 4, 100);
            float y = v.Value.pos.y;
            if (check(y, time) || check(y, time - 0.04f))
            {
                score++;
            }
           
        }
        Debug.Log(count + ":" + score);
        return score / count;
    }

    private bool check(float y, float time)
    {
        if (bounceCSV.data.ContainsKey(time.ToString()) && cmpWithStandard(y, time))
        {

            return true;
        }
        return false;
    }

    private bool cmpWithStandard(float y, float time)
    {
        float standardY = bounceCSV.data[time.ToString()];
        if (Mathf.Abs(y - standardY) <= 0.0001)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
