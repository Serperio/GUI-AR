using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CanvasFlipper : MonoBehaviour
{
    //Campos de error en Canvas Error
    [SerializeField]
    TextMeshProUGUI gps;
    [SerializeField]
    TextMeshProUGUI wifiOn;
    [SerializeField]
    TextMeshProUGUI Datos;
    [SerializeField]
    TextMeshProUGUI conexionInternet;

    //Canvas Bueno = canvas con la interfaz ; Canvas Malo = Canvas con errores
    [SerializeField]
    GameObject canvasBueno;
    [SerializeField]
    GameObject canvasMalo;

    void Update(){
    if(gps.text== "Si se ha activado el GPS" && (Datos.text=="Si hay datos"   || conexionInternet.text == "Hay internet por wifi") && wifiOn.text== "WiFi esta activado")
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
