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
    public List<Vecino> Vecinos { get; set; }
    //public double Distancia { get; set; }
    public Puntito Anterior { get; set; }

    public Puntito(string id, double latitud2, double longitud2)
    {
        ID = id;
        latitud = latitud2;
        longitud = longitud2;
        Vecinos = new List<Vecino>();
        //Distancia = double.MaxValue;
        Anterior = null;
    }
}

public class Vecino
{
    public Puntito Actual { get; set; }
    public Puntito Vec { get; set; }
    public double Distancia { get; set; }

    public Vecino(Puntito actual, Puntito vecino, double dist)
    {
        Actual = actual;
        Vec = vecino;
        Distancia = dist;
    }
}

class Dijkstra
{
    public static List<Puntito> FindShortestPath(List<Puntito> vertices, Puntito punto_inicial, Puntito final)
    {   
        List<Puntito> caminos = new List<Puntito>();
        caminos.Add(punto_inicial);
        if(punto_inicial == final) return caminos;
        
        //punto_inicial.Distancia = 0;

        HashSet<Puntito> visitados = new HashSet<Puntito>();
        HashSet<Puntito> no_visitados = new HashSet<Puntito>(vertices);
    
        Puntito aux = punto_inicial;
        no_visitados.Remove(aux);
        visitados.Add(aux);
        //Debug.Log("Inicio: " + aux.ID);

        while (no_visitados.Count > 0)
        {
            if(HayPuntoFinal(aux.Vecinos, final))
            {
                Debug.Log("Punto A: " + aux.ID + " ---> Punto B: " + final.ID);
                aux = final;
                caminos.Add(aux);
                visitados.Add(aux);
                no_visitados.Remove(aux);
                //Debug.Log("LLEGAMOS AL FINAL: " + aux.ID);
                break;
            }
            else
            {
                Puntito actual = GetPointWithMinDistancia(aux.Vecinos, aux, visitados);
                // Debug.Log("Punto Actual: " + actual.ID);
                no_visitados.Remove(actual);
                visitados.Add(actual);
                //Debug.Log("Actual es: " + actual.ID);
                //caminos.Add(actual);

                foreach (Vecino vecinito in actual.Vecinos)
                {
                    Puntito vecino = vecinito.Vec;
                    //double DistanciaFromInicio = actual.Distancia + vecinito.Distancia;

                    if (!visitados.Contains(vecino))
                    {   //  && DistanciaFromInicio < vecino.Distancia
                        //vecino.Distancia = DistanciaFromInicio;
                        vecino.Anterior = actual;
                        //visitados.Add(vecino);
                        //Debug.Log("Punto: " + aux.ID);
                        caminos.Add(actual);
                        aux = actual;
                    }
                }
            }
        }
        //Debug.Log("Ya no hay No Visitados");
        return caminos;
    }

    private static Puntito GetPointWithMinDistancia(List<Vecino> Vecinos, Puntito Actual, HashSet<Puntito> visitados)
    {
        Puntito minPoint = null;
        double minDistancia = double.MaxValue;

        foreach (Vecino vecino in Vecinos)
        {
            if (DistanciaLatLon.distance(vecino.Vec.latitud, Actual.latitud, vecino.Vec.longitud, Actual.longitud) < minDistancia && !visitados.Contains(vecino.Vec))
            {
                minPoint = vecino.Vec;
                minDistancia = DistanciaLatLon.distance(vecino.Vec.latitud, Actual.latitud, vecino.Vec.longitud, Actual.longitud);
            }
        }
        return minPoint;
    }

    private static bool HayPuntoFinal(List<Vecino> Vecinos, Puntito Final)
    {
        foreach(Vecino vecino in Vecinos){
            if(vecino.Vec == Final) return true;
        }
        return false;
    }
}

/* public class Matrix{
    public static double[,] Crear_Matrix(List<Puntito> vertices)
    {
        double[,] matrix = new double[vertices.Count,vertices.Count];
        for (int i = 0; i < vertices.Count; i++)
        {
            Puntito Actual = vertices[i];
            matrix[i,i] = 0;
            for(int j = 0; j < Actual.Vecinos.Count; j++)
            {
                matrix[i+1,j] = DistanciaLatLon.distance(Actual.latitud, // Latitud Nodo actual
                                                     Actual.Vecinos[j].Vec.latitud, // Latitud Punto Vecino
                                                     Actual.longitud, // Longitud Punto actual
                                                     Actual.Vecinos[j].Vec.longitud); // Longitud Punto Vecino
            }
        }
        return matrix;
    }
} */

public class CreateRuta : MonoBehaviour
{   
    public TextMeshProUGUI texto_punto;
    
    void Start() 
    {
        /* Puntito A = new Puntito("A", -33.04373, -71.63103);
        Puntito B = new Puntito("B", -33.04372, -71.63104);
        Puntito C = new Puntito("C", -33.04372, -71.63105);
        Puntito D = new Puntito("D", -33.04371, -71.63105); */ 

        Puntito A = new Puntito("A", -33.04373, -71.63103);
        Puntito B = new Puntito("B", -33.04372, -71.63104);
        Puntito C = new Puntito("C", -33.04372, -71.63105);
        Puntito D = new Puntito("D", -33.04371, -71.63105);
        Puntito E = new Puntito("E", -33.04373, -71.63105);
        Puntito F = new Puntito("F", -33.04371, -71.63104);

        List<Puntito> vertices = new List<Puntito> { A, B, C, D, E, F};
        //double[,] matrix = Matrix.Crear_Matrix(vertices);

        /* ConectarPuntosAleatoriamente(A, B);
        ConectarPuntosAleatoriamente(A, C);
        ConectarPuntosAleatoriamente(B, C);
        ConectarPuntosAleatoriamente(B, D);
        ConectarPuntosAleatoriamente(C, D);
        ConectarPuntosAleatoriamente(E, F); */

        // A
        A.Vecinos.Add(new Vecino(A,B,DistanciaLatLon.distance(A.latitud,B.latitud,A.longitud,B.longitud)));
        A.Vecinos.Add(new Vecino(A,C,DistanciaLatLon.distance(A.latitud,C.latitud,A.longitud,C.longitud)));
        // B
        B.Vecinos.Add(new Vecino(B,C,DistanciaLatLon.distance(B.latitud,C.latitud,B.longitud,C.longitud)));
        B.Vecinos.Add(new Vecino(B,D,DistanciaLatLon.distance(B.latitud,D.latitud,B.longitud,D.longitud)));
        B.Vecinos.Add(new Vecino(B,A,DistanciaLatLon.distance(B.latitud,A.latitud,B.longitud,A.longitud)));
        // C
        C.Vecinos.Add(new Vecino(C,B,DistanciaLatLon.distance(C.latitud,B.latitud,C.longitud,B.longitud)));
        C.Vecinos.Add(new Vecino(C,A,DistanciaLatLon.distance(C.latitud,A.latitud,C.longitud,A.longitud)));
        C.Vecinos.Add(new Vecino(C,D,DistanciaLatLon.distance(C.latitud,D.latitud,C.longitud,D.longitud)));
        // D
        D.Vecinos.Add(new Vecino(D,B,DistanciaLatLon.distance(D.latitud,B.latitud,D.longitud,B.longitud)));
        D.Vecinos.Add(new Vecino(D,C,DistanciaLatLon.distance(D.latitud,C.latitud,D.longitud,C.longitud)));
        D.Vecinos.Add(new Vecino(D,E,DistanciaLatLon.distance(D.latitud,E.latitud,D.longitud,E.longitud)));
        // E
        E.Vecinos.Add(new Vecino(E,D,DistanciaLatLon.distance(E.latitud,D.latitud,E.longitud,D.longitud)));
        E.Vecinos.Add(new Vecino(E,F,DistanciaLatLon.distance(E.latitud,F.latitud,E.longitud,F.longitud)));
        // F
        F.Vecinos.Add(new Vecino(F,E,DistanciaLatLon.distance(F.latitud,E.latitud,F.longitud,E.longitud)));

        List<Puntito> camino = Dijkstra.FindShortestPath(vertices,C,F);
        if(camino.Count > 0)
        {
            foreach(Puntito punto in camino)
            {   
                Debug.Log("Punto: " + punto.ID +" Ahora iremos a:");
                texto_punto.text += "Punto " + punto.ID + "---------> ";
            }
        } 
        else 
        {
            texto_punto.text = "No se encontro camino factible";
        }
        
    }
    void update()
    {

    }
}
