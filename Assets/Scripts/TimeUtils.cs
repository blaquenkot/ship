using UnityEngine;

public class TimeUtils
{
    public static string GetTimeAsString(float time)
    {
        return Mathf.Floor(time / 60).ToString("00") + ':' + (time % 60).ToString("00");
    }
}
