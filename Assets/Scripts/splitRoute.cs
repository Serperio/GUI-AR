using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class splitRoute : MonoBehaviour
{

    // Ruta a mostrar
    [SerializeField]
    public List<Point> rutaEjemplo;
    //Point punto= new Point();

    //Texto a cambiar
    [SerializeField]
    TextMeshProUGUI textoEnInterfazRutas;

    //public GameObject bottonInfo;

    string listadoRutas;
    void Start()
    {
        rutaEjemplo = new List<Point>();
        //punto.name = "Patio";
        //rutaEjemplo.Add(punto);
        getRoute();
    }

    public void getRoute()
    {
        listadoRutas = "";
        foreach (Point point in rutaEjemplo)
        {
            listadoRutas += point.name + "\n\n";
        }
        textoEnInterfazRutas.text = listadoRutas;
    }
}
