using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Networking;
using TMPro;

public class Wifi : MonoBehaviour
{

    /*
     * Obtiene redes WIFI disponibles (cercanas)
     * Actualizar los valores de WIFI en script API
     * 
     */

    //int buttonTries = 0;

    [SerializeField]
    API api;
    //Obtener WIFI Manager
    private static AndroidJavaObject unityActivity;
    private static AndroidJavaObject _wifiManager;
    private const string WIFI_ACCESS = "android.permission.ACCESS_WIFI_STATE";
    private const string FINE_LOCATION = "android.permission.ACCESS_FINE_LOCATION";

    //Colocar ip puerto del servidor
    //string IP = "144.22.42.236";
    //string port = "3000";

    //Listado redes WIFI
    [SerializeField]
    TextMeshProUGUI redes;

    //Lista de redes WIFI
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
                //Obtienes WIFI disponibles (cercanas)
                redes.text = "Obteniendo resultados";
                AndroidJavaObject scanResults = _wifiManager.Call<AndroidJavaObject>("getScanResults");
                int wifiCount = scanResults.Call<int>("size");
                redes.text = "Wifis: "+wifiCount.ToString();
                //Parsea los datos importantes de la lista obtenida en scanResults
                for (int i = 0; i < wifiCount; i++)
                {
                    AndroidJavaObject scanResult = scanResults.Call<AndroidJavaObject>("get", i);
                    int signalStrength = _wifiManager.CallStatic<int>("calculateSignalLevel", scanResult.Get<int>("level"), 20);
                    string SSID = scanResult.Get<string>("BSSID");
                    Network network = new Network();
                    network.SSID = SSID;
                    network.signalLevel = signalStrength;
                    salida += "SSID: " + SSID + "| Level: " + signalStrength + "\n";
                    networks.Add(network);
                }
                //Agrega los datos en GetWiFis
                api.getWifis(networks);
                redes.text = "" + wifiCount + "already";
                StartCoroutine(UpdatorWifis());
                //redes.text = salida+"Finished";
            }
            else
            {
                Permission.RequestUserPermission(FINE_LOCATION);
                Debug.Log("Pedir permisos Location");
                ScanWifi();
            }

        }
        else
        {
            Permission.RequestUserPermission(WIFI_ACCESS);
            Debug.Log("Pedir permisos wifi");
            ScanWifi();
        }
    }
    public IEnumerator UpdatorWifis()
    {
        string salida = "";
        _wifiManager.Call<bool>("setWifiEnabled", false);
        yield return new WaitForSeconds(10f);
        _wifiManager.Call<bool>("setWifiEnabled", true);
        _wifiManager.Call<bool>("startScan");

        redes.text = "Pidiendo permisos fine location";
        // Vaciar lista de redes wifi para poder empezar escaneo desde cero
        networks = new List<Network>();
        //Obtienes WIFI disponibles (cercanas)
       _wifiManager.Call<AndroidJavaObject>("getScanResults");
        AndroidJavaObject scanResults = _wifiManager.Call<AndroidJavaObject>("getScanResults");
        int wifiCount = scanResults.Call<int>("size");
        redes.text = "Obteniendo resultados enumerator";
        //Parsea los datos importantes de la lista obtenida en scanResults
        redes.text = "Contadas: "+wifiCount.ToString();
        for (int i = 0; i < wifiCount; i++)
        {
            AndroidJavaObject scanResult = scanResults.Call<AndroidJavaObject>("get", i);
            int signalStrength = _wifiManager.CallStatic<int>("calculateSignalLevel", scanResult.Get<int>("level"), 20);
            string SSID = scanResult.Get<string>("BSSID");
            Network network = new Network();
            network.SSID = SSID;
            network.signalLevel = signalStrength;
            salida += "SSID: " + SSID + "| Level: " + signalStrength + "\n";
            networks.Add(network);
        }
        redes.text = salida;
        //Agrega los datos en GetWiFis
        api.getWifis(networks);
        yield return new WaitForSeconds(20f);
        StartCoroutine(UpdatorWifis());
    }
}
