using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeUtil
{
    public static float Round(float time, int basei, int precision)
    {
        int tmpTime = Mathf.RoundToInt(time * precision);
        if (tmpTime % basei == 0)
        {
            return time;
        }
        else
        {
            int multiple = (tmpTime / basei) * basei;
            float newTime = (float)multiple / (float)precision;
            return newTime;
        }
    }
}
