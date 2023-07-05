using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using TMPro;

public class MyPositionGPS : MonoBehaviour
{
    private bool hasLocationPermission;
    private LocationInfo lastLocation;
    List<Punto> puntos = new List<Punto>();

    [SerializeField]
    TextMeshProUGUI texto;
    [SerializeField]
    TextMeshProUGUI textoPuntos;

    public class Punto
    {
        public float longitud;
        public float latitud;
        public int piso;
        public float distanciaActual;
        public Punto(float latitudAux, float longitudAux, int pisoAux, float distanciaActualAux) {
            longitud = longitudAux;
            latitud = latitudAux;
            distanciaActual = distanciaActualAux;
            piso = pisoAux;
        }
    }


    void Start()
    {
        puntos.Add(new Punto(-33.03479f, -71.59643f, 2,1000f));
        puntos.Add(new Punto(-33.03481f, -71.59651f, 2, 1000f));
        puntos.Add(new Punto(-33.03503f, -71.5967f, 2, 1000f));//CAMBIAR AL CANNON

        puntos.Add(new Punto(0f, 0f, 1,1000f));
        printLista(puntos);

        // Verificar y solicitar permiso de ubicaci�n (solo necesario para Android)
        if (Application.platform == RuntimePlatform.Android)
        {
            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                Permission.RequestUserPermission(Permission.FineLocation);
            }
        }

        // Comenzar la obtenci�n de la ubicaci�n
        StartCoroutine(StartGPS());
    }
    private System.Collections.IEnumerator StartGPS()
    {
        // Esperar hasta que se otorgue el permiso de ubicaci�n (solo necesario para Android)
        while (Application.platform == RuntimePlatform.Android && !Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            yield return null;
        }

        // Iniciar la obtenci�n de la ubicaci�n
        Input.location.Start();

        // Esperar hasta que se obtenga una ubicaci�n v�lida
        while (Input.location.status == LocationServiceStatus.Initializing)
        {
            yield return null;
        }

        // Verificar si se pudo obtener la ubicaci�n
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("No se pudo obtener la ubicaci�n del GPS.");
            yield break;
        }

        // Obtener la ubicaci�n actualizada
        lastLocation = Input.location.lastData;
        Input.location.Stop();
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(StartGPS());

    }

    void Update()
    {

        // Obtener la posici�n actualizada si est� disponible
        //if (Input.location.status == LocationServiceStatus.Running)
        //{
        //    Input.location.Start();
        //lastLocation = Input.location.lastData;
        //    Input.location.Stop();
        //}
        texto.text = "Mi Posicion de GPS es (" + lastLocation.latitude + "," + lastLocation.longitude + ") ";
        if (puntos.Count != 0)
        {
            puntos[0].distanciaActual = Mathf.Sqrt(Mathf.Pow((lastLocation.latitude - puntos[0].latitud), 2) + Mathf.Pow((lastLocation.longitude - puntos[0].longitud), 2));
            texto.text +=  " con una distancia de: " + puntos[0].distanciaActual;
        }
        if (puntos.Count == 0)
        {
            textoPuntos.text = "No quedan puntos";
        }
        else if (puntos[0].distanciaActual <6e-5f)
        {
            puntos.RemoveAt(0);
            printLista(puntos);
        }

        //puntos[1].distanciaActual = Mathf.Sqrt(Mathf.Pow((lastLocation.latitude - puntos[1].latitud), 2) - Mathf.Pow((lastLocation.longitude - puntos[1].longitud), 2));
    }

    void OnApplicationQuit()
    {
        // Detener el servicio de ubicaci�n al salir de la aplicaci�n
        Input.location.Stop();
    }
    void printLista(List<Punto> puntos)
    {
        textoPuntos.text = "";
        foreach(Punto punto in puntos)
        {
            textoPuntos.text += "latitud: " + punto.latitud + ", longitud: " + punto.longitud + ", piso: " + punto.piso + "\n";
        }
    }
}
