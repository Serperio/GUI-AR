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
    [SerializeField]
    GameObject actualizarRutaButton;
    [SerializeField]
    ClosestPoint closest;
    private bool recalculado = false;

    [SerializeField]
    SimpleAR simpleAR;
    
    private void Start()
    {
        // Revisar si se tiene que modificar la ruta
        StartCoroutine(ModificarRuta());
    }
    
    private void Update()
    {
        if(ultimaRuta != null && ultimaRuta.Count <= 2)
        {
            actualizarRutaButton.SetActive(false);
        }
        // Quitar punto de la ruta si ya llegue
        if (ultimaRuta != null && closest.lastPosition != null && ultimaRuta.Count >= 2 && closest.lastPosition.name == ultimaRuta[1].nombre)
        {
            ultimaRuta.RemoveAt(1);
        }

    }
    
    IEnumerator ModificarRuta()
    {
        float[] position = myPositionGPS.GetLastPosition();
        float[] sigPunto = { 0,0};
        if (ultimaRuta != null && ultimaRuta.Count >= 2)
        {
            sigPunto[0] =  (float)ultimaRuta[1].latitud;
            sigPunto[1] = (float)ultimaRuta[1].longitud;
        }
        if(closest.lastPosition != null)
        {
            if(closest.lastPosition.name == "@Escalera2" && !recalculado) {
                Utilities._ShowAndroidToastMessage("Recalculando ruta");
                generarRuta(lastName);
                recalculado = true;
            }  
        }
        /*
        if (distanciaCoord(position, sigPunto) >= 0.05)
        {
            generarRuta(lastName);
            yield return new WaitForSeconds(5); // esperar a que se genere la nueva ruta
        }
        */
        yield return new WaitForSeconds(2); // esperar para ver si tiene que actualizar
        StartCoroutine(ModificarRuta());
    }

    public float distanciaCoord(float[] p1, float[] p2)
    {
        return Mathf.Sqrt(Mathf.Pow(p1[0]-p2[0], 2)+ Mathf.Pow(p1[1]- p2[1], 2)) * 100; //0.01 grados de diferencia es mas o menos 1 kilometro
    }

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
                    Debug.Log("No existe ruta con escaleras permitidas");
                    popUpNoExisteRuta.SetActive(true);
                }
                else
                {
                    try
                    {
                        ruta = Dijkstra.FindShortestPath(auxiliar, auxiliar[0], auxiliar[auxiliarFinalCamino], true); //Cambiar a puntos recuperados escaleras
                        canvaWarning.SetActive(true);
                        Debug.Log("Existe ruta con escaleras");
                    }
                    catch
                    {
                        Debug.Log("No existe ruta con escaleras");
                        foreach (Puntito puntito in auxiliar)
                        {
                            Debug.Log("Punto:"+ puntito.nombre);
                        }
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
            //rutaCustomAPI.LoadRoute(ruta);
            simpleAR.LoadRoute(ruta);
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
        auxiliar = new List<Puntito> { new Puntito("inicio", latitud, longitud, "inicio") };
        List<Puntito> vertices = new List<Puntito> { inicio }; // indice 0 corresponde siempre al inicio

        // Recorrer puntos recuperados para generar lista de vertices
        int auxiliarActual = 1;
        int verticeActual = 1; // indice del vertice actual para no tener que buscarlo
        foreach (Point punto in puntos)
        {
            // Puntos permitido para probar para evitar que el algoritmo falle
            //if (punto.name == "@Labux" || punto.name == "@Escalera1" || punto.name == "@Escalera2" || punto.name == "@P223" || punto.name == "@Ascensor")
            if (punto.name == "patio" || punto.name == "pieza" || punto.name == "plaza")
            {
                // Filtro de puntos prohibidos
                if (puntosProhibidos.Contains(punto.name)) continue;
                // Saltarse las escaleras
                if (escalerasPermitidas == false && punto.tipo == "Escalera") {
                    auxiliar.Add(new Puntito(punto._id, punto.x, punto.y, punto.name));
                    Debug.Log("punto especial:" + punto.name);
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
                                auxiliar[auxiliarActual].Vecinos.Add(new Vecino(auxiliar[auxiliarActual], auxiliar[indiceVecino]));
                                auxiliar[indiceVecino].Vecinos.Add(new Vecino(auxiliar[indiceVecino], auxiliar[auxiliarActual]));
                            }
                        }
                    }
                    Debug.Log("Vecinos especial:" + auxiliar[auxiliarActual].Vecinos.Count);
                    auxiliarActual++;
                    continue;
                };
                // Crear el vertice
                vertices.Add(new Puntito(punto._id, punto.x, punto.y, punto.name));
                auxiliar.Add(new Puntito(punto._id, punto.x, punto.y, punto.name));
                // Revisar si tiene vecinos
                if (punto.vecinos.Length > 0)
                {
                    Debug.Log("Vecinos normal antes:" + punto.vecinos.Length);
                    // Agregar vecinos
                    foreach (string vecinoID in punto.vecinos)
                    {
                        // Encontrar indice del vecino en la lista de vertices
                        int indiceVecino = getIndexFromListByID(vertices, vecinoID);
                        int indiceVecinoAux = getIndexFromListByID(auxiliar, vecinoID);
                        if (indiceVecino != -1)
                        {
                            // Agregar vecinos para ambos lados
                            vertices[verticeActual].Vecinos.Add(new Vecino(vertices[verticeActual], vertices[indiceVecino]));
                            vertices[indiceVecino].Vecinos.Add(new Vecino(vertices[indiceVecino], vertices[verticeActual]));
                        }
                        if (indiceVecinoAux != -1)
                        {
                            auxiliar[auxiliarActual].Vecinos.Add(new Vecino(auxiliar[auxiliarActual], auxiliar[indiceVecinoAux]));
                            auxiliar[indiceVecinoAux].Vecinos.Add(new Vecino(auxiliar[indiceVecinoAux], auxiliar[auxiliarActual]));
                        }
                    }
                    Debug.Log("Vecinos normal aux:" + auxiliar[auxiliarActual].nombre + "|" + auxiliar[auxiliarActual].Vecinos.Count);
                }
                auxiliarActual++;
                verticeActual++; // Pasar al siguiente vertice en la lista
            }
        }

        // TODO: Pedir punto mas cercano
        Point lastPos =  closest.lastPosition;
        // agregar vecino del punto inicial al punto mas cercano

        //if(lastPos != null)
        //{
        /*
        int indicePuntoCercano = getIndexFromListByName(vertices, lastPos.name);
        int indicePuntoCercanoAux = getIndexFromListByName(auxiliar, lastPos.name);
        */

        float[] lastPosition = { latitud, longitud };
        int indicePuntoCercano = closestPointIndex(vertices, lastPosition );
        int indicePuntoCercanoAux = closestPointIndex(auxiliar, lastPosition);

        Debug.Log("Unity indice: " + indicePuntoCercano);

        // agregar vecino del punto inicial al punto mas cercano
        /*
        int indicePuntoCercano = getIndexFromListByName(vertices, "@Labux");
        int indicePuntoCercanoAux = getIndexFromListByName(auxiliar, "@Labux");
        */
        vertices[0].Vecinos.Add(new Vecino(vertices[0], vertices[indicePuntoCercano]));
        auxiliar[0].Vecinos.Add(new Vecino(auxiliar[0], auxiliar[indicePuntoCercanoAux]));
        vertices[indicePuntoCercano].Vecinos.Add(new Vecino(vertices[indicePuntoCercano], vertices[0]));
        auxiliar[indicePuntoCercanoAux].Vecinos.Add(new Vecino(auxiliar[indicePuntoCercanoAux], auxiliar[0]));
        rutaCustomAPI.puntosConEscaleras = auxiliar;
        //}
        return vertices;
    }

    public int closestPointIndex(List<Puntito> puntos, float[] position)
    {
        int minIndex = -1;
        int index = 0;
        float minDist = float.PositiveInfinity;
        foreach(Puntito punto in puntos)
        {
            float distanceBetweenPoints = distance((float)punto.latitud, (float)punto.longitud, position[0], position[1]);
            if (distanceBetweenPoints < minDist && index != 0)
            {
                minDist = distanceBetweenPoints;
                minIndex = index;
            }
            index++;
        }
        return minIndex;
    }

    private float distance(float lat1, float long1, float lat2, float long2)
    {
        return Vector2.Distance(new Vector2(lat1, long1), new Vector2(lat2, long2));
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
