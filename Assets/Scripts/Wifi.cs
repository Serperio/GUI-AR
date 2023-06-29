using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Networking;
using TMPro;

public class Wifi : MonoBehaviour
{
    [SerializeField]
    API api;

    private static AndroidJavaObject unityActivity;
    private static AndroidJavaObject _wifiManager;
    private const string WIFI_ACCESS = "android.permission.ACCESS_WIFI_STATE";
    private const string FINE_LOCATION = "android.permission.ACCESS_FINE_LOCATION";
    [SerializeField]
    string IP = "146.235.246.2";
    [SerializeField]
    string port = "3000";

    [SerializeField]
    TextMeshProUGUI redes;

    private List<Network> networks;

    private void Start()
    {
        unityActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        networks = new List<Network>();
        ScanWifi();
    }

    public void ScanWifi()
    {
        string salida = "";
        redes.text = "Escaneando";
        if (Permission.HasUserAuthorizedPermission(WIFI_ACCESS))
        {
            redes.text = "Pidiendo permisos";
            Debug.Log("Permisos de WIFI");
            if (Permission.HasUserAuthorizedPermission(FINE_LOCATION))
            {
                redes.text = "Pidiendo permisos fine location";
                _wifiManager = unityActivity.Call<AndroidJavaObject>("getSystemService", "wifi");
                AndroidJavaObject scanResults = _wifiManager.Call<AndroidJavaObject>("getScanResults");
                int wifiCount = scanResults.Call<int>("size");
                redes.text = "Obteniendo resultados";
                for(int i = 0; i < wifiCount; i++)
                {
                    AndroidJavaObject scanResult = scanResults.Call<AndroidJavaObject>("get", i);
                    int signalStrength = _wifiManager.CallStatic<int>("calculateSignalLevel", scanResult.Get<int>("level"), 5);
                    string SSID = scanResult.Get<string>("BSSID");
                    Network network = new Network();
                    network.SSID = SSID;
                    network.signalLevel = signalStrength;
                    salida += "SSID: " + SSID + "| Level: " + signalStrength + "\n";
                    networks.Add(network);
                }
                api.getWifis(networks);
                //StartCoroutine(Upload());
                redes.text = salida;
                
            } else {
                Permission.RequestUserPermission(FINE_LOCATION);
                Debug.Log("Pedir permisos Location");
            }

        }
        else
        {
            Permission.RequestUserPermission(WIFI_ACCESS);
            Debug.Log("Pedir permisos wifi");
        }
    }


    IEnumerator Upload()
    {
        WWWForm form = new WWWForm();
        foreach(Network net in networks)
        {
            form.AddField(net.SSID, net.signalLevel);
        }
        form.AddField("end","true");
        UnityWebRequest www = UnityWebRequest.Post("http://"+IP+":"+port+"/post", form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error post: "+ www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
        }
    }
}
