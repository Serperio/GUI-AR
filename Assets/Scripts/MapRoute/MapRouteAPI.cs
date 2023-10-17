using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRouteAPI : MonoBehaviour
{
    [SerializeField]
    // Codigo para generar rutas custom por codigo
    ARLocation.MapboxRoutes.RutaCustomAPI rutaCustomAPI;
    [SerializeField]
    MyPositionGPS myPositionGPS;

    public void generarRuta(string name)
    {
        // Recuperar datos desde la api
        StartCoroutine(APIHelper.GET("points", response => {
            // Obtener listado de puntos en formato List<Point> a partir de json
            List<Point> puntosRecuperados = pointsFromJsonList(response);

            float[] position = myPositionGPS.GetLastPosition();
            // Convertir de List<Point> a formato de Dijkstra
            List<Puntito> vertices = puntitosFromPointList(puntosRecuperados, position[0], position[1]);
            
            // Encontrar punto final
            int verticeFinalCamino = getIndexFromListByName(vertices, name);

            // Generar ruta de Dijkstra
            /*
            List<Puntito> ruta = Dijkstra.FindShortestPath(vertices, vertices[0], vertices[verticeFinalCamino]);

            // Imprimir ruta
            string textoRuta = "";
            foreach(Puntito punto in ruta)
            {
                textoRuta += punto.nombre+"->";
            }
            printf(textoRuta);
            */
            List<Puntito> ruta = new List<Puntito>();
            // Cargar ejecutar ruta en mapbox
            rutaCustomAPI.LoadRoute(ruta);
        }));
    }

    private List<Puntito> puntitosFromPointList(List<Point> puntos, float latitud, float longitud)
    {
        // Definir punto inicial
        Puntito inicio = new Puntito("inicio", latitud, longitud, "inicio");
        // Lista para guardar los vertices/puntos en formato Dijkstra
        List<Puntito> vertices = new List<Puntito> { inicio }; // indice 0 corresponde siempre al inicio

        // Recorrer puntos recuperados para generar lista de vertices
        int verticeActual = 1; // indice del vertice actual para no tener que buscarlo
        foreach (Point punto in puntos)
        {
            if(punto.name == "@Labux" || punto.name == "@Escalera1" || punto.name == "@P223") // Puntos permitido para probar
            {
                // Crear el vertice
                vertices.Add(new Puntito(punto._id, punto.x, punto.y, punto.name));
                // Revisar si tiene vecinos
                if (punto.vecinos.Length > 0)
                {
                    // Agregar vecinos
                    foreach (string vecinoID in punto.vecinos)
                    {
                        // Encontrar indice del vecino en la lista de vertices
                        int indiceVecino = getIndexFromListByID(vertices, vecinoID);
                        if (indiceVecino != -1)
                        {
                            // Agregar vecinos para ambos lados
                            vertices[verticeActual].Vecinos.Add(new Vecino(vertices[verticeActual], vertices[indiceVecino]));
                            vertices[indiceVecino].Vecinos.Add(new Vecino(vertices[indiceVecino], vertices[verticeActual]));
                        }
                    }
                }
                verticeActual++; // Pasar al siguiente vertice en la lista
            }
        }
        // agregar vecino del punto inicial al punto mas cercano
        int indicePuntoCercano = getIndexFromListByName(vertices, "@Escalera1");
        vertices[0].Vecinos.Add(new Vecino(vertices[0], vertices[indicePuntoCercano]));
        vertices[indicePuntoCercano].Vecinos.Add(new Vecino(vertices[indicePuntoCercano], vertices[0]));

        return vertices;
    }

    private int getIndexFromListByName(List<Puntito> puntos, string nombre)
    {
        for(int i = 0; i < puntos.Count; i++)
        {
            if(puntos[i].nombre == nombre)
            {
                return i;
            }
        }
        return -1;
    }

    private int getIndexFromListByID(List<Puntito> puntos, string id)
    {
        for (int i = 0; i < puntos.Count; i++)
        {
            if (puntos[i].ID == id)
            {
                return i;
            }
        }
        return -1;
    }

    private List<Point> pointsFromJsonList(string jsonList)
    {
        List<string> puntosRecuperados = API.listJson(jsonList);
        //Convertir json a punto
        List<Point> puntos = new List<Point>();
        foreach (string jsonPoint in puntosRecuperados)
        {
            puntos.Add(JsonUtility.FromJson<Point>(jsonPoint));
        }
        return puntos;
    }

    private void printf(string s)
    {
        print("@" + s);
    }
}
