using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.NetworkInformation;
using TMPro;

public class WifiChecker : MonoBehaviour
{
    //Detecta si el Wifi de android esta activado o no , conectable con WifiDetection

    //Campo de error
    [SerializeField]
    TextMeshProUGUI wifiActivate;

    private bool isWifiOff = false;
    //private bool isWifiConnected = false;
    //private bool isInternetConnected = false;

    void FixedUpdate()
    {
        // Verificar si el Wi-Fi está desactivado
        isWifiOff = IsWifiOff();
        if (isWifiOff)
        {
            wifiActivate.text = "Wi-Fi está desactivado.";
        }
        else
        {
            wifiActivate.text = "WiFi esta activado";
        }
    }


    private bool IsWifiOff()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaClass contextClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject context = contextClass.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject wifiManager = context.Call<AndroidJavaObject>("getSystemService", "wifi");
            return !(bool)wifiManager.Call<bool>("isWifiEnabled");
#else
        return false;
#endif
    }

}
