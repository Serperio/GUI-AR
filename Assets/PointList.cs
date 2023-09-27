using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointList : MonoBehaviour
{
    [SerializeField]
    MyPositionGPS myPositionGPS;
    [SerializeField]
    Info_Rutas info_Rutas;

    //Declaracion de parametros de un punto
    public class Punto
    {
        public double longitud;
        public double latitud;
        public int piso;
        public string nombre;
        public Punto(double latitudAux, double longitudAux, int pisoAux, string nombreaux)
        {
            longitud = longitudAux;
            latitud = latitudAux;
            nombre = nombreaux;
            piso = pisoAux;
        }
    }
    List<Punto> caminoEnPuntos;


    public void GetPath(string name)
    {
        caminoEnPuntos = new List<Punto>();
        float[] init_point_aux= myPositionGPS.GetLastPosition(); //Array latitude and longitude [0] [1]
        Puntito init_point = new Puntito("Inicial", init_point_aux[0], init_point_aux[1]); 
        Puntito last_point= new Puntito("Error", 0,0);
        StartCoroutine(APIHelper.GET("points", response => {
        List<Puntito> vertices=new List<Puntito> {init_point};
            // Obtener listado de puntos
            List<string> data = API.listJson(response);
            // Transformar JSON a Point
            List<Point> points = new List<Point>();
            foreach (string dato in data)
            {
                points.Add(JsonUtility.FromJson<Point>(dato));
            }
            // Entregar resultados
            foreach (Point point in points)
            {
                //Los demas puntos :)
                if (point.name == name)
                {
                    last_point = new Puntito(name, point.x, point.y);
                    vertices.Add(last_point);
                }
                else
                {
                    vertices.Add(new Puntito(name, point.x, point.y));
                }
            }
            init_point.Vecinos.Add(new Vecino(init_point, last_point));
            List<Puntito> camino = Dijkstra.FindShortestPath(vertices, init_point, last_point);
            foreach (Puntito puntito in camino)
            {
                caminoEnPuntos.Add(new Punto(puntito.latitud, puntito.longitud, 0, puntito.ID));
            }
            print(printPath(caminoEnPuntos));
        }));


    }
    string printPath(List<Punto> lista)
    {
        string res="El camino:";
        foreach (Punto punto in lista)
        {
            res += punto.nombre + "->";
        }
        return res;
    }
}
