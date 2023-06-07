using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PluginInit : MonoBehaviour
{
    AndroidJavaClass unityClass;
    AndroidJavaObject unityActivity;
    AndroidJavaObject _pluginInstance;

    // Start is called before the first frame update
    void Start()
    {
        InitializePlugin("com.drktech.wifipluginlib.PluginInstance");
    }


    void InitializePlugin(string pluginName)
    {
        unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        unityActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
        _pluginInstance = new AndroidJavaObject(pluginName);
        if(_pluginInstance == null)
        {
            Debug.Log("Error plugin init");
        }
        _pluginInstance.CallStatic("receiveActivity", unityActivity);
    }

    public void Toast()
    {
        if(_pluginInstance != null)
        {
            _pluginInstance.Call("Toast", "Unity!");
        }
    }
}
