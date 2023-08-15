using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
using TMPro;
/*
A partir de una matriz de puntos, genera la ruta mas corta de A, hasta B
 */
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

    public Vecino(Puntito actual, Puntito vecino)
    {
        Actual = actual;
        Vec = vecino;
        Distancia = DistanciaLatLon.distance(actual.latitud,vecino.latitud,actual.longitud,vecino.longitud);
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
                        if(!caminos.Contains(actual))
                        {
                            caminos.Add(actual);
                        }
                        
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

        /* Puntito A = new Puntito("A", -33.04373, -71.63103);
        Puntito B = new Puntito("B", -33.04372, -71.63104);
        Puntito C = new Puntito("C", -33.04372, -71.63105);
        Puntito D = new Puntito("D", -33.04371, -71.63105);
        Puntito E = new Puntito("E", -33.04373, -71.63105);
        Puntito F = new Puntito("F", -33.04371, -71.63104); */


        //double[,] matrix = Matrix.Crear_Matrix(vertices);

        /* ConectarPuntosAleatoriamente(A, B);
        ConectarPuntosAleatoriamente(A, C);
        ConectarPuntosAleatoriamente(B, C);
        ConectarPuntosAleatoriamente(B, D);
        ConectarPuntosAleatoriamente(C, D);
        ConectarPuntosAleatoriamente(E, F); */
/* 
        // A
        A.Vecinos.Add(new Vecino(A,B));
        A.Vecinos.Add(new Vecino(A,C));
        // B
        B.Vecinos.Add(new Vecino(B,C));
        B.Vecinos.Add(new Vecino(B,D));
        B.Vecinos.Add(new Vecino(B,A));
        // C
        C.Vecinos.Add(new Vecino(C,B));
        C.Vecinos.Add(new Vecino(C,A));
        C.Vecinos.Add(new Vecino(C,D));
        // D
        D.Vecinos.Add(new Vecino(D,B));
        D.Vecinos.Add(new Vecino(D,C));
        D.Vecinos.Add(new Vecino(D,E));
        // E
        E.Vecinos.Add(new Vecino(E,D));
        E.Vecinos.Add(new Vecino(E,F));
        // F
        F.Vecinos.Add(new Vecino(F,E)); */
        
        // A


    }

    public void BuscaRuta(int ruta){
        Debug.Log("Ruta: "+ruta);
        Puntito A = new Puntito("Labpro", -33.03479f, -71.59643f);
        Puntito B = new Puntito("Escalera1", -33.03481f, -71.59651f);
        Puntito C = new Puntito("Cañon", -33.04373, -71.63103);

        A.Vecinos.Add(new Vecino(A,B));
        B.Vecinos.Add(new Vecino(B,C));
        B.Vecinos.Add(new Vecino(B,A));
        C.Vecinos.Add(new Vecino(C,B));

        List<Puntito> vertices = new List<Puntito> { A, B, C};
        List<Puntito> camino;

        if(ruta == 0){
            camino = Dijkstra.FindShortestPath(vertices,A,C);
        }
        else {
            camino = Dijkstra.FindShortestPath(vertices,C,A);
        }
        //List<Puntito> camino = Dijkstra.FindShortestPath(vertices,A,C);
        if(camino.Count > 0)
        {
            
            texto_punto.text = "Ruta: - ";
            foreach(Puntito punto in camino)
            {   
                Debug.Log("Punto: " + punto.ID +" Ahora iremos a:");
                texto_punto.text += punto.ID + " -> ";
            }
        } 
        else 
        {
            texto_punto.text = "No se encontro camino factible";
        }
    }
}
