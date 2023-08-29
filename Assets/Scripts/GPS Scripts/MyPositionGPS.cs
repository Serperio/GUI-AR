using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using TMPro;

public class MyPositionGPS : MonoBehaviour
{

    /*
     Toma una lista de puntos para seguir como camino,
     Los puntos estan harcodeados
     */

    private bool hasLocationPermission;
    private LocationInfo lastLocation;

    private float _lastLatitude;
    private float _lastLongitude;

    //Lista con los puntos que quedan por recorrer
    List<Punto> puntos = new List<Punto>();

    
    [SerializeField]
    TextMeshProUGUI posicionActual;

    [SerializeField]
    TextMeshProUGUI puntosFaltantes;

    //Variables donde se guarda el error que muestra el programa
    float horizontalAccuracy;
    float verticalAccuracy;

    //Campo donde se mostrara el margen de rror
    [SerializeField]
    TextMeshProUGUI Error;


    //Declaracion de parametros de un punto
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
        puntos.Add(new Punto(-33.03503f, -71.5967f, 2, 1000f));

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
        // Esperar hasta que se otorgue el permiso de ubicacion (solo necesario para Android)
        while (Application.platform == RuntimePlatform.Android && !Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            yield return new WaitForSeconds(0.5f);
            //StartCoroutine(StartGPS());
        }

        // Iniciar la obtencion de la ubicacion
        Input.location.Start();

        // Esperar hasta que se obtenga una ubicacion valida
        while (Input.location.status == LocationServiceStatus.Initializing)
        {
            yield return new WaitForSeconds(0.5f);
            //StartCoroutine(StartGPS());
        }

        // Verificar si se pudo obtener la ubicacion
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("No se pudo obtener la ubicaci�n del GPS.");
            yield return new WaitForSeconds(0.5f);
            //StartCoroutine(StartGPS());
        }

        //Calculo de error
        horizontalAccuracy = Input.location.lastData.horizontalAccuracy * 0.46f;
        verticalAccuracy = Input.location.lastData.verticalAccuracy * 0.46f;
        //Mostrar error
        Error.text = "Vert. Relative Error: " + verticalAccuracy.ToString() + " [m]\nHoriz. Relative Error: " + horizontalAccuracy.ToString() + " [m]";
        
        // Obtener la ubicacion actualizada
        lastLocation = Input.location.lastData;
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(StartGPS());

    }

    void Update()
    {
        //Revisa constantemente que tal alejado se esta del punto
        _lastLatitude = lastLocation.latitude;
        _lastLongitude = lastLocation.longitude;
        posicionActual.text = "Mi Posicion de GPS es (" + lastLocation.latitude + "," + lastLocation.longitude + ") ";
        if (puntos[0].distanciaActual <6e-5f)
        {
            puntos.RemoveAt(0);
            printLista(puntos);
        }
    }


    public float[] GetLastPosition()
    {
        float[] position = { _lastLatitude, _lastLongitude };
        return position;
    }

    void OnApplicationQuit()
    {
        // Detener el servicio de ubicaci�n al salir de la aplicacion
        Input.location.Stop();
    }
    void printLista(List<Punto> puntos) //Imprime la lista de puntos en pantalla
    {
        puntosFaltantes.text = "";
        foreach(Punto punto in puntos)
        {
            puntosFaltantes.text += "latitud: " + punto.latitud + ", longitud: " + punto.longitud + ", piso: " + punto.piso + "\n";
        }
    }
}
