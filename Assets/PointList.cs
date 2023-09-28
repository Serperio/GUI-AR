using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ARLocation.MapboxRoutes
{

    public class PointList : MonoBehaviour
    {
        [SerializeField]
        MyPositionGPS myPositionGPS;
        [SerializeField]
        Info_Rutas info_Rutas;
        [SerializeField]
        RoutetoAR construirRuta;
        //Declaracion de parametros de un punto

        List<Point> caminoEnPuntos;


        public void GetPath(string name)
        {
            caminoEnPuntos = new List<Point>();
            float[] init_point_aux = myPositionGPS.GetLastPosition(); //Array latitude and longitude [0] [1]
            Puntito init_point = new Puntito("Inicial", init_point_aux[0], init_point_aux[1]);
            Puntito last_point = new Puntito("Error", 0, 0);
            StartCoroutine(APIHelper.GET("points", response =>
            {
                List<Puntito> vertices = new List<Puntito> { init_point };
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
                    caminoEnPuntos.Add(new Point((float)puntito.latitud, (float)puntito.longitud, puntito.ID));
                }
                print(printPath(caminoEnPuntos));
                construirRuta.initiateRoute(caminoEnPuntos);
            }));


        }
        string printPath(List<Point> lista)
        {
            string res = "El camino:";
            foreach (Point punto in lista)
            {
                res += punto.name + "->";
            }
            return res;
        }
    }
}
