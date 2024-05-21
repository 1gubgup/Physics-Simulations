using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class CSVUtil : MonoBehaviour
{
    public string readResult { get; private set; }
    public Dictionary<string, float> data { get; private set; }
    
    public IEnumerator GetData(string filename)
    {
        var uri = new System.Uri(Path.Combine(Application.streamingAssetsPath, filename));
        UnityWebRequest www = UnityWebRequest.Get(uri);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            //Debug.Log(www.downloadHandler.text);
            readResult = www.downloadHandler.text;
            data = new Dictionary<string, float>();
            data = GetDictionary();
        }
    }

    private Dictionary<string, float> GetDictionary()
    {
        Dictionary<string, float> data = new Dictionary<string, float>();
        string[] pairs = readResult.Split('\n');
        for(int i = 0; i< pairs.Length-1; i++)
        {
            string pair = pairs[i];
            string[] tmp = pair.Split(',');
            //string k = tmp[0];
            //float key = Convert.ToSingle(tmp[0].Trim());
            ////string v = tmp[1].TrimEnd('\r');
            float value = Convert.ToSingle(tmp[1].TrimEnd('\r'));
            data[tmp[0]] = value;
        }
        //foreach (var v in data)
        //{
        //    Debug.Log(v.Key + ": " + v.Value);
        //}
        return data;
    }
}
