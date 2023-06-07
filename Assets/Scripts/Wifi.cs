using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Networking;

public class Wifi : MonoBehaviour
{
    private static AndroidJavaObject unityActivity;
    private static AndroidJavaObject _wifiManager;
    private const string WIFI_ACCESS = "android.permission.ACCESS_WIFI_STATE";
    private const string FINE_LOCATION = "android.permission.ACCESS_FINE_LOCATION";
    [SerializeField]
    string IP = "146.235.246.2";
    [SerializeField]
    string port = "3000";
    public struct Network { public string SSID; public int signalLevel; }

    private List<Network> networks;

    private void Start()
    {
        unityActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        networks = new List<Network>();
    }

    public void ScanWifi()
    {
        if (Permission.HasUserAuthorizedPermission(WIFI_ACCESS))
        {
            Debug.Log("Permisos de WIFI");
            if (Permission.HasUserAuthorizedPermission(FINE_LOCATION))
            {
                _wifiManager = unityActivity.Call<AndroidJavaObject>("getSystemService", "wifi");
                AndroidJavaObject scanResults = _wifiManager.Call<AndroidJavaObject>("getScanResults");
                int wifiCount = scanResults.Call<int>("size");
                for(int i = 0; i < wifiCount; i++)
                {
                    AndroidJavaObject scanResult = scanResults.Call<AndroidJavaObject>("get", i);
                    int signalStrength = _wifiManager.CallStatic<int>("calculateSignalLevel", scanResult.Get<int>("level"), 5);
                    string SSID = scanResult.Get<string>("SSID");
                    Network network = new Network();
                    network.SSID = SSID;
                    network.signalLevel = signalStrength;
                    Debug.Log("SSID: " + SSID + "| Level: " + signalStrength);
                    networks.Add(network);
                }
                StartCoroutine(Upload());
                
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
