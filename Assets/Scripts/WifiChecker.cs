using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.NetworkInformation;
using TMPro;

public class WifiChecker : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI texto;
    private bool isWifiOff = false;
    private bool isWifiConnected = false;
    private bool isInternetConnected = false;

    void Start()
    {
        // Verificar si el Wi-Fi est� desactivado
        isWifiOff = IsWifiOff();

        // Verificar si hay una conexi�n Wi-Fi activa
        isWifiConnected = IsWifiConnected();

        // Verificar si hay una conexi�n a Internet
        isInternetConnected = IsInternetConnected();

        // Imprimir el resultado
        if (isWifiOff)
        {
            Debug.Log("Wi-Fi est� desactivado.");
            texto.text = "Wi-Fi est� desactivado.";
        }
        else if (isWifiConnected && isInternetConnected)
        {
            Debug.Log("Wi-Fi est� activado y conectado a Internet.");
            texto.text = "Wi-Fi est� activado y conectado a Internet.";
        }
        else if (isWifiConnected && !isInternetConnected)
        {
            Debug.Log("Wi-Fi est� activado, pero no est� conectado a Internet.");
            texto.text = "Wi-Fi est� activado, pero no est� conectado a Internet.";
        }
        else
        {
            Debug.Log("Wi-Fi est� activado, pero no se ha detectado ninguna conexi�n.");
            texto.text = "Wi-Fi est� activado, pero no se ha detectado ninguna conexi�n.";
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

    private bool IsWifiConnected()
    {
        return NetworkInterface.GetIsNetworkAvailable() && Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
    }

    private bool IsInternetConnected()
    {
        return NetworkInterface.GetIsNetworkAvailable() && Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
    }
}
