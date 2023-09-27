using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DetalleBoton : MonoBehaviour
{
    //Harcodea la ruta del canon a la aplicacion

    Buscador buscador;    
    TextMeshProUGUI punto;
    CreateRuta ruta;
    PointList listaPunto;
 
    void Start(){
        listaPunto = GameObject.Find("CreadorStringRuta").GetComponent<PointList>();
        punto = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        buscador = GameObject.Find("AppManager").GetComponent<Buscador>();
        ruta = GameObject.Find("AppManager").GetComponent<CreateRuta>();
    }
    public void Detalle(){
        listaPunto.GetPath(punto.text);
        Debug.Log("Ha pasado!");
        buscador.DetalleSitio(punto);
        if(punto.text == "Cañon"){
            ruta.BuscaRuta(0);
        }
        else{
            ruta.BuscaRuta(1);
        }
        
    }
    
}
