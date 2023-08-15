using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.NetworkInformation;
using TMPro;

public class WifiDetection : MonoBehaviour
{
   /*
   Este Script detecta si se cumplen los requisitos para que funcione la aplicacion, 
   es decir, revisa: GPS, WIFI activado y Conexion a internet.
   Si hay errores mostrara un canvas con errores, en otro caso, mostrara la aplicacion.
   */

    //Booleanos accesibles que muestran el estado de cada parametro
    public bool enabledGPS; //Si el GPS esta encendido
    public bool enabledWiFiSearch; //Si el Wifi esta encendido
    public bool enabledMobileInternet; //Si hay internet por medio de Datos moviles
    public bool enabledWiFiInternet; //Si hay internet por medio de WiFi

    //Textos reescritos en el menu de errores
    [SerializeField]
    TextMeshProUGUI gpsText; //Error de GPS
    [SerializeField]
    TextMeshProUGUI wifiActivate; //Error de wifi
    [SerializeField]
    TextMeshProUGUI wifiConected; //Error de conexion a internet
    [SerializeField]
    TextMeshProUGUI mobileData; //Error Datos moviles a internet

    //Canvas Bueno = canvas con la interfaz ; Canvas Malo = Canvas con errores
    [SerializeField]
    GameObject canvasBueno;
    [SerializeField]
    GameObject canvasMalo;

    private void Start()
    {
        LocationService service = new LocationService(); //Crea un servicio (Ni idea como funciona)

        StartCoroutine(updateOff(service)); //Comprobacion que se realiza cada 5 segundos
    }

    IEnumerator updateOff(LocationService service)
    {

        //Revisa Buscador de Wifis
        enabledWiFiSearch = !IsWifiOff();
        wifiActivate.text = enabledWiFiSearch ? "WiFi esta activado" : "Wi-Fi está desactivado.";

        //Revisa GPS
        enabledGPS = service.isEnabledByUser;
        gpsText.text = enabledGPS ? "Si se ha activado el GPS" : "No ha sido activado el GPS";

        //Revisa Internet por WIFI
        enabledWiFiInternet = Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
        wifiConected.text = enabledWiFiInternet ? "Hay internet por wifi" : "No hay internet por wifi";

        //Revisa internet por Datos
        enabledMobileInternet = Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork;
        mobileData.text = enabledMobileInternet ? "Si hay datos" : "No hay datos";

        //Si encuentra algun error en la iteracion, cambiara el canvas, si se ha arreglado, mostrara el otro
        if (enabledGPS && (enabledMobileInternet || enabledWiFiInternet) && enabledWiFiSearch)
        {
            canvasBueno.SetActive(true);
            canvasMalo.SetActive(false);
        }
        else
        {
            canvasBueno.SetActive(false);
            canvasMalo.SetActive(true);
        }

        yield return new WaitForSeconds(1.0f);
        StartCoroutine(updateOff(service));
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
