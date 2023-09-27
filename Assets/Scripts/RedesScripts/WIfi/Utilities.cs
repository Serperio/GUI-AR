using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    public static int SortByMac(WifiData w1, WifiData w2)
    {
        return w1.mac.CompareTo(w2.mac);
    }

    public static int SortByFloor(WifiData w1, WifiData w2)
    {
        return w1.floor.CompareTo(w2.floor);
    }

    public static void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, message, 0);
                toastObject.Call("show");
            }));
        }
    }
}
