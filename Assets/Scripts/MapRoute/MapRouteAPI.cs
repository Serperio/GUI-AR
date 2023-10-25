using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapRouteAPI : MonoBehaviour
{
    [SerializeField]
    // Codigo para generar rutas custom por codigo
    ARLocation.MapboxRoutes.RutaCustomAPI rutaCustomAPI;
    [SerializeField]
    MyPositionGPS myPositionGPS;
    [SerializeField]
    TextMeshProUGUI watchRoute;
    private List<Puntito> ultimaRuta;
    private List<string> puntosProhibidos = new List<string>();
    private string lastName = "";
    private bool escalerasPermitidas = true;
    List<Puntito> auxiliar;
    [SerializeField]
    GameObject canvaWarning;

    [SerializeField]
    GameObject popUpNoExisteRuta;


    public void generarRuta(string name)
    {
        lastName = name;
        // Recuperar datos desde la api
        StartCoroutine(APIHelper.GET("points", response =>
        {
            // Obtener listado de puntos en formato List<Point> a partir de json
            List<Point> puntosRecuperados = pointsFromJsonList(response);

            float[] position = myPositionGPS.GetLastPosition();
            // Convertir de List<Point> a formato de Dijkstra
            List<Puntito> vertices = puntitosFromPointList(puntosRecuperados, position[0], position[1]);

            // Encontrar punto final
            int verticeFinalCamino = getIndexFromListByName(vertices, name);
            int auxiliarFinalCamino = getIndexFromListByName(auxiliar, name);

            // Generar ruta de Dijkstra
            List<Puntito> ruta = new List<Puntito>();
            try
            {
                ruta = Dijkstra.FindShortestPath(vertices, vertices[0], vertices[verticeFinalCamino]);
            }
            catch
            {
                if (escalerasPermitidas)
                {
                popUpNoExisteRuta.SetActive(true);
                }
                else
                {
                    try
                    {
                        ruta = Dijkstra.FindShortestPath(auxiliar, auxiliar[0], auxiliar[auxiliarFinalCamino],true); //Cambiar a puntos recuperados escaleras
                        canvaWarning.SetActive(true);
                    }
                    catch
                    {
                        popUpNoExisteRuta.SetActive(true);
                    }
                }
            }
            // Imprimir ruta
            string textoRuta = "";
            foreach(Puntito punto in ruta)
            {
                textoRuta += punto.nombre+"->";
            }

            printf(textoRuta);
            watchRoute.text = textoRuta;

            //List<Puntito> ruta = new List<Puntito>();

            // Guardar ultima ruta encontrada
            ultimaRuta = ruta;

            // Cargar ejecutar ruta en mapbox
            rutaCustomAPI.LoadRoute(ruta);
        }));
    }

    public List<Puntito> getLastRoute()
    {
        return ultimaRuta;
    }

    public void LoadPuntosProhibidos(List<string> puntos)
    {
        puntosProhibidos = puntos;
        generarRuta(lastName);
        
    }

    private List<Puntito> puntitosFromPointList(List<Point> puntos, float latitud, float longitud)
    {
        // Definir punto inicial
        Puntito inicio = new Puntito("inicio", latitud, longitud, "inicio");
        // Lista para guardar los vertices/puntos en formato Dijkstra
        auxiliar = new List<Puntito> { inicio };
        List<Puntito> vertices = new List<Puntito> { inicio }; // indice 0 corresponde siempre al inicio

        // Recorrer puntos recuperados para generar lista de vertices
        int auxiliarActual = 1;
        int verticeActual = 1; // indice del vertice actual para no tener que buscarlo
        foreach (Point punto in puntos)
        {
            // Puntos permitido para probar para evitar que el algoritmo falle
            if (punto.name == "@Labux" || punto.name == "@Escalera1" || punto.name == "@Escalera2" || punto.name == "@P223") 
            {
                // Filtro de puntos prohibidos
                if (puntosProhibidos.Contains(punto.name)) continue;
                // Saltarse las escaleras
                if (escalerasPermitidas == false && punto.tipo == "Escalera") {
                    auxiliar.Add(new Puntito(punto._id, punto.x, punto.y, punto.name));
                    // Revisar si tiene vecinos
                    if (punto.vecinos.Length > 0)
                    {
                        // Agregar vecinos
                        foreach (string vecinoID in punto.vecinos)
                        {
                            // Encontrar indice del vecino en la lista de vertices
                            int indiceVecino = getIndexFromListByID(auxiliar, vecinoID);
                            if (indiceVecino != -1)
                            {
                                // Agregar vecinos para ambos lados
                                auxiliar[verticeActual].Vecinos.Add(new Vecino(auxiliar[verticeActual], auxiliar[indiceVecino]));
                                auxiliar[indiceVecino].Vecinos.Add(new Vecino(auxiliar[indiceVecino], auxiliar[verticeActual]));
                            }
                        }
                    }
                    auxiliarActual++;
                    continue;
                };
                // Crear el vertice
                vertices.Add(new Puntito(punto._id, punto.x, punto.y, punto.name));
                auxiliar.Add(new Puntito(punto._id, punto.x, punto.y, punto.name));
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
                            auxiliar[verticeActual].Vecinos.Add(new Vecino(auxiliar[verticeActual], auxiliar[indiceVecino]));
                            auxiliar[indiceVecino].Vecinos.Add(new Vecino(auxiliar[indiceVecino], auxiliar[verticeActual]));
                            // Agregar vecinos para ambos lados
                            vertices[verticeActual].Vecinos.Add(new Vecino(vertices[verticeActual], vertices[indiceVecino]));
                            vertices[indiceVecino].Vecinos.Add(new Vecino(vertices[indiceVecino], vertices[verticeActual]));
                        }
                    }
                }
                auxiliarActual++;
                verticeActual++; // Pasar al siguiente vertice en la lista
            }
        }
        // agregar vecino del punto inicial al punto mas cercano
        int indicePuntoCercano = getIndexFromListByName(vertices, "@Labux");
        vertices[0].Vecinos.Add(new Vecino(vertices[0], vertices[indicePuntoCercano]));
        auxiliar[0].Vecinos.Add(new Vecino(auxiliar[0], auxiliar[indicePuntoCercano]));
        vertices[indicePuntoCercano].Vecinos.Add(new Vecino(vertices[indicePuntoCercano], vertices[0]));
        auxiliar[indicePuntoCercano].Vecinos.Add(new Vecino(auxiliar[indicePuntoCercano], auxiliar[0]));
        rutaCustomAPI.puntosConEscaleras = auxiliar;
        return vertices;
    }

    public void setEscalerasPermitidas (bool permitido)
    {
        escalerasPermitidas = permitido;
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
