using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Networking;
using TMPro;
using System.Globalization;

namespace ARLocation.MapboxRoutes
{
    public class DetalleBoton : MonoBehaviour
    {
        //Harcodea la ruta del canon a la aplicacion

        Buscador buscador;
        TextMeshProUGUI punto;
        CreateRuta ruta;
        PointList listaPunto;
        MapRouteAPI mapRouteAPI;

        GameObject canva;

        [SerializeField]
        API api;
        [SerializeField]
        TextMeshProUGUI mensajeFinal;
        [SerializeField]
        GameObject PanelDBError;
        private LocationInfo lastLocation;
        private float lastLatitude;
        private float lastLongitude; 
        System.DateTime currentDateTime;
        MyPositionGPS gpsloc;
        public List<Point> pointList1;
        public List<Point> pointList;
        public string puntoInicial;

        void Start()
        {
            
            //listaPunto = GameObject.Find("CreadorStringRuta").GetComponent<PointList>();
            punto = gameObject.GetComponentInChildren<TextMeshProUGUI>();
            buscador = GameObject.Find("AppManager").GetComponent<Buscador>();
            ruta = GameObject.Find("AppManager").GetComponent<CreateRuta>();
            mapRouteAPI = GameObject.Find("AppManager").GetComponent<MapRouteAPI>();
            PanelDBError = GameObject.Find("PanelErrorDBBuscado");
            mensajeFinal = GameObject.Find("TextoErrorDB").GetComponent<TextMeshProUGUI>(); 
            gpsloc = GameObject.Find("AppManager").GetComponent<MyPositionGPS>();
            api = GameObject.Find("AppManager").GetComponent<API>();
        }
        public void Detalle()
        {
            //listaPunto.GetPath(punto.text);
            Debug.Log("Ha pasadoadasdasdasasasddasads! "+punto.text);
            //ObtenerInfoRuta(punto.text);
            buscador.DetalleSitio(punto);
            /*
            if (punto.text == "Cañon")
            {
                ruta.BuscaRuta(0);
            }
            else
            {
                ruta.BuscaRuta(1);
            }*/

        }

// -------------------------------------------------------------------------------- //
// -------------------------------------------------------------------------------- //

    public void ObtenerInfoRuta(string nombrepuntoFinal)  
    {
        Input.location.Start();
        lastLocation = Input.location.lastData;
        lastLatitude = lastLocation.latitude;
        lastLongitude = lastLocation.longitude;
        currentDateTime = System.DateTime.Now;
        puntoInicial = NearestPointName();
        Debug.Log("Punto final aaaaaa wea wena: "+nombrepuntoFinal);
        StartCoroutine(SendPuntosRuta(lastLatitude, lastLongitude, puntoInicial, nombrepuntoFinal, currentDateTime));
        Input.location.Stop();
    }

    public string NearestPointName()
    {
        GameObject.Find("AppManager").GetComponent<API>().NearbyPointsAPI();
        float[] positions = gpsloc.GetLastPosition();
            Debug.Log("Posicion:" + positions[0]);
        float latitude = positions[0]; //Pasar
        float longitude = positions[1]; //Pasar de gps
        Point nearestPoint = null;
        double shortestDistance = Mathf.Infinity;
        pointList1 = api._pointlist;

        pointList = new List<Point>();

        foreach (Point point in pointList1)
        {
            if (point.floor.ToString() == api.pred.prediction.ToString())
            {
                pointList.Add(point);
            }
        }

        foreach (Point point in pointList)
        {
            double distance = CalculateDistance(latitude, longitude, point.x, point.y);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestPoint = point;
            }
        }
        if (nearestPoint == null) return "No";
        return nearestPoint.name;
    }

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        double distance = Mathf.Sqrt(Mathf.Abs((float)((lat2 - lat1) * (lat2 - lat1) + (lon2 - lon1) * (lon2 - lon1))));
        return distance;
    }

    IEnumerator SendPuntosRuta(float latitud, float longitud,string PuntoInicial, string PuntoFinal, System.DateTime Fecha)
    {
        //const string IP = "144.22.42.236";
        //Debug.Log("Inicio: "+PuntoInicial+"\nfinal: "+PuntoFinal+"\Fecha+hora: "+Fecha);
        const string IP = "144.22.42.236";
        const string port = "3000";
        const string baseURI = "http://"+IP+":"+port+"/api/";
        // Crear formulario
        WWWForm form = new WWWForm();
        form.AddField("puntoInicial", PuntoInicial);
        form.AddField("puntoFinal", PuntoFinal);
        form.AddField("longitud", longitud.ToString(CultureInfo.InvariantCulture));
        form.AddField("latitud", latitud.ToString(CultureInfo.InvariantCulture));
        form.AddField("fecha", Fecha.ToString("dddd, dd MMMM yyyy hh:mm tt"));
        //Realizar request
        UnityWebRequest www = UnityWebRequest.Post(baseURI+"puntobuscado/add", form);
        yield return www.SendWebRequest();
        // Resolucion de la request
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error post: "+ www.error);
            mensajeFinal.text = "Error al Mandar Estadisticas";
            PanelDBError.SetActive(true);
        }
        else
        {
            mensajeFinal.text = "Estadisticas Enviadas!!";
            Debug.Log("Form upload complete!");
        }
        yield break;
    }

// -------------------------------------------------------------------------------- //
// -------------------------------------------------------------------------------- //
        public void RealizarRuta()
        {

            //listaPunto.GetPath(punto.text);
            mapRouteAPI.generarRuta(punto.text);
            ObtenerInfoRuta(punto.text);
            canva = GameObject.Find("Canva_GenerarRuta");
            canva.SetActive(false);
        }
    }

}
