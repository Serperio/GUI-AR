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
    void FixedUpdate()
    {
        LocationService service = new LocationService();
        gpsText.text = service.isEnabledByUser ? "Si se ha activado el GPS" : "No ha sido activado el GPS";
        print(Application.internetReachability);
        wifiConected.text = Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork ? "Hay internet por wifi" : "No hay internet por wifi";
        mobileData.text = Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork ? "Si hay datos" : "No hay datos";
    }
}
