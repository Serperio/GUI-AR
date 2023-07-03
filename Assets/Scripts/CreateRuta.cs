using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
using TMPro;

public class Puntito
{
    public string ID { get; set; }
    public double latitud { get; set; }
    public double longitud { get; set; }
    public List<Edge> Edges { get; set; }
    public double Distancia { get; set; }
    public Puntito Anterior { get; set; }

    public Puntito(string id, double latitud2, double longitud2)
    {
        ID = id;
        latitud = latitud2;
        longitud = longitud2;
        Edges = new List<Edge>();
        Distancia = double.MaxValue;
        Anterior = null;
    }
}

public class Edge
{
    public Puntito Inicio { get; set; }
    public Puntito Fin { get; set; }
    public double Peso { get; set; }

    public Edge(Puntito inicio, Puntito final, double peso)
    {
        Inicio = inicio;
        Fin = final;
        Peso = peso;
    }
}

public class CreateRuta : MonoBehaviour
{   
    public TextMeshProUGUI texto_punto;
    
    void Start() 
    {
        Puntito A = new Puntito("A", -33.04373, -71.63103);
        Puntito B = new Puntito("B", -33.04372, -71.63104);
        Puntito C = new Puntito("C", -33.04372, -71.63105);
        Puntito D = new Puntito("D", -33.04371, -71.63105);

        List<Puntito> vertices = new List<Puntito> { A, B, C, D};

        foreach (Puntito point in vertices)
        {
            texto_punto.text += "ID: " + point.ID + ", Latitud: " + point.latitud + "\n";
        }
    }
    void update()
    {

    }
}
