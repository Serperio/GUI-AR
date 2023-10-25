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

        List<Point> caminoEnPuntos;

        ClosestPoint csp;

        public void GetPath(string name)
        {
            caminoEnPuntos = new List<Point>();
            float[] init_point_aux = myPositionGPS.GetLastPosition(); //Array latitude and longitude [0] [1]
            Puntito init_point = new Puntito("Inicial", init_point_aux[0], init_point_aux[1]);
            init_point.latitud = -33.04028;
            init_point.longitud = -71.59032;
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
                    if (point.name == name) // Punto final
                    {
                        Debug.Log("guardando last point");
                        last_point = new Puntito(point._id, point.x, point.y, point.name);
                        vertices.Add(last_point);
                        string[] auxVecinos = point.vecinos;
                        foreach (string a in auxVecinos)
                        {
                            foreach (Puntito b in vertices)
                            {
                                if (b.ID == a)
                                {
                                    last_point.Vecinos.Add(new Vecino(b, last_point));
                                    b.Vecinos.Add(new Vecino(b, last_point));
                                }
                            }
                        }
                    }
                    else
                    {
                        // Crear punto
                        Puntito puntitoAux = new Puntito(point._id, point.x, point.y, point.name);
                        vertices.Add(puntitoAux);
                        // Crear vecinos
                        string[] auxVecinos = point.vecinos;
                        foreach (string a in auxVecinos)
                        {
                            foreach (Puntito b in vertices)
                            {
                                if (b.ID == a)
                                {
                                    puntitoAux.Vecinos.Add(new Vecino(b, puntitoAux));
                                    b.Vecinos.Add(new Vecino(b, puntitoAux));
                                }
                            }
                        }
                    }
                }
                // Unir punto actual con punto mas cercano
                foreach (Puntito auxPuntito in vertices)
                {
                    if (auxPuntito.nombre == "pieza")
                    {
                        init_point.Vecinos.Add(new Vecino(init_point, auxPuntito));
                    }
                }
                // imprimir vecinos de cada punto
                foreach (Puntito auxprint in vertices)
                {
                    string aux = "";
                    foreach (Vecino vecino in auxprint.Vecinos)
                    {
                        aux += vecino.Actual.nombre;
                    }
                    print(auxprint.nombre + ":" + aux);
                }
                print("@"+vertices[vertices.Count - 1].Vecinos[0].Actual.nombre);
                //init_point.Vecinos.Add(new Vecino(init_point, last_point)); //->Jarcodear init point a un punto existente
                //Debug.Log("@"+vertices.Count);
                List<Puntito> camino = Dijkstra.FindShortestPath(vertices, init_point, last_point);
                if(camino != null && camino.Count > 0)
                {
                    foreach (Puntito puntito in camino)
                    {
                        caminoEnPuntos.Add(new Point((float)puntito.latitud, (float)puntito.longitud, puntito.nombre));
                    }
                    print(printPath(caminoEnPuntos));
                
                    construirRuta.initiateRoute(caminoEnPuntos);
                }
                else
                {
                    Debug.Log("NO HAY CAMINO, FLACO, DESPEJE");
                }
            }));
            Debug.Log("APUNTO DE SUBIR DUMMYS");
            csp.EnviarDataPuntoInteres("dummy44", (float)-33.03518, (float)-71.59681);
            Debug.Log("DUMMYS subidossssssssssssssssssssssssss");
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
