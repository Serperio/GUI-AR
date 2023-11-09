using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CanvasFlipper : MonoBehaviour
{
    //Se encarga de cambiar entre el canvas que posee error y el que no según sea el estado del mismo

    [SerializeField]
    WifiDetection wifiDetection;

    //Canvas Bueno = canvas con la interfaz ; Canvas Malo = Canvas con errores
    [SerializeField]
    GameObject canvasBueno;
    [SerializeField]
    GameObject canvasMalo;

    void Update(){
    if(wifiDetection.enabledGPS && (wifiDetection.enabledMobileInternet   || wifiDetection.enabledWiFiInternet) && wifiDetection.enabledWiFiSearch)
        {
        canvasBueno.SetActive(true);
        canvasMalo.SetActive(false);
    }
    else{
        canvasBueno.SetActive(false);
        canvasMalo.SetActive(true);
    }
    }
}
