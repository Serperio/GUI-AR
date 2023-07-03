using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WifiDetection : MonoBehaviour
{

    //Deteccion de conexion a internet, wifi y datos, conectable con WifiChecker
    [SerializeField]
    TextMeshProUGUI gpsText;
    [SerializeField]
    TextMeshProUGUI wifiConected;
    [SerializeField]
    TextMeshProUGUI mobileData;
    [SerializeField]
    TextMeshProUGUI esperandoWifi;

    private void Start()
    {
        LocationService service = new LocationService();
        StartCoroutine(updateOff(service));
    }

    IEnumerator updateOff(LocationService service)
    {
        if (!(Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork || Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork))
        {
            esperandoWifi.text = "Esperando Conexión";
        }
        yield return new WaitForSeconds(5.0f);
        esperandoWifi.text = "";
        gpsText.text = service.isEnabledByUser ? "Si se ha activado el GPS" : "No ha sido activado el GPS";
        wifiConected.text = Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork ? "Hay internet por wifi" : "No hay internet por wifi";
        mobileData.text = Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork ? "Si hay datos" : "No hay datos";
        StartCoroutine(updateOff(service));

    }
}
