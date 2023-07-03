using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CanvasFlipper : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI csm;
    [SerializeField]
    TextMeshProUGUI wifiOn;
    [SerializeField]
    TextMeshProUGUI Datos;
    [SerializeField]
    GameObject canvasBueno;
    [SerializeField]
    GameObject canvasMalo;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
if(csm.text== "Si se ha activado el GPS" && (Datos.text=="Si hay datos"   || wifiOn.text == "Si se ha activado el Wifi"))
{
    canvasBueno.SetActive(true);
    canvasMalo.SetActive(false);
}
else
{
    canvasBueno.SetActive(false);
    canvasMalo.SetActive(true);
}
    }
}
