using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DetalleBoton : MonoBehaviour
{
    Buscador buscador;    
    TextMeshProUGUI punto;
 
    void Start(){
        punto = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        buscador = GameObject.Find("BuscadorManager").GetComponent<Buscador>();
    }

    public void Detalle(){
        buscador.DetalleSitio(punto);
    }
}
