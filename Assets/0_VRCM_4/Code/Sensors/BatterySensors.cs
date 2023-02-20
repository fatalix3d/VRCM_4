using UnityEngine;
using System;
using System.Collections;

public class BatterySensors
{
    public BatterySensors()
    {

    }

    public static string GetBatteryLevel()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            using (var javaUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (var currentActivity = javaUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    using (var androidPlugin = new AndroidJavaObject("spxyz.com.androidsensors.SensorsData", currentActivity))
                    {
                        return androidPlugin.Call<string>("GetBatteryPct");
                    }
                }
            }
        }

        return "nope";
    }
}
