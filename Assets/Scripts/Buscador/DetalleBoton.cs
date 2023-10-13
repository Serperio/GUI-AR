using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace ARLocation.MapboxRoutes
{
    public class DetalleBoton : MonoBehaviour
    {
        //Harcodea la ruta del canon a la aplicacion

        Buscador buscador;
        TextMeshProUGUI punto;
        CreateRuta ruta;
        PointList listaPunto;
        MapRouteAPI mapRouteAPI;

        GameObject canva;
        void Start()
        {
            
            //listaPunto = GameObject.Find("CreadorStringRuta").GetComponent<PointList>();
            punto = gameObject.GetComponentInChildren<TextMeshProUGUI>();
            buscador = GameObject.Find("AppManager").GetComponent<Buscador>();
            ruta = GameObject.Find("AppManager").GetComponent<CreateRuta>();
            mapRouteAPI = GameObject.Find("AppManager").GetComponent<MapRouteAPI>();
        }
        public void Detalle()
        {
            //listaPunto.GetPath(punto.text);
            Debug.Log("Ha pasado! "+punto.text);
            buscador.DetalleSitio(punto);
            /*
            if (punto.text == "Cañon")
            {
                ruta.BuscaRuta(0);
            }
            else
            {
                ruta.BuscaRuta(1);
            }*/

        }

        public void RealizarRuta()
        {

            //listaPunto.GetPath(punto.text);
            mapRouteAPI.generarRuta(punto.text);
            canva = GameObject.Find("Canva_GenerarRuta");
            canva.SetActive(false);
        }
    }
}
