using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCharts;

public struct Pair
{
    public string key;
    public string value;
    public Pair(string k, string v)
    {
        key = k;
        value = v;
    }
}

public class lineChartController : MonoBehaviour
{
    public string chartName;
    private LineChart chart;

    private void Awake()
    {
        chart = gameObject.GetComponent<LineChart>();
        chart.title.show = true;
        chart.title.text = chartName;
        chart.xAxises[0].type = Axis.AxisType.Category;
        chart.yAxises[0].type = Axis.AxisType.Value;
        chart.xAxises[0].splitNumber = 5;
        chart.xAxises[0].boundaryGap = false;
        chart.RemoveData();
        chart.AddSerie(SerieType.Line);
    }

    public void transformData(Dictionary<float, float> dict)
    {
        chart.RemoveData();
        chart.AddSerie(SerieType.Line);
        foreach (var v in dict)
        {
            chart.AddXAxisData(v.Key.ToString());
            chart.AddData(0, v.Value);
        }
    }

    public List<Pair> getChartData()
    {
        List<Pair> chartValues = new List<Pair>();
        int count = chart.xAxis0.data.Count;
        Debug.Log(count);
        List<SerieData> data = chart.series.list[0].data;
        for(int i = 0; i<count; i++)
        {
            string x = chart.xAxis0.data[i];
            float y = data[i].data[1];
            chartValues.Add(new Pair(x,y.ToString()));
        }
        return chartValues;
    }
}
