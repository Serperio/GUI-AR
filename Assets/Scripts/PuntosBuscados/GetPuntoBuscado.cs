using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Networking;
using TMPro;

public class GetPuntoBuscado : MonoBehaviour
{
    [SerializeField]
    string puntoFinal;
    [SerializeField]
    API api;
    [SerializeField]
    TextMeshProUGUI mensajeFinal;
    private LocationInfo lastLocation;
    private float lastLatitude;
    private float lastLongitude; 
    System.DateTime currentDateTime;
    MyPositionGPS gpsloc;
    public List<Point> pointList1;
    public List<Point> pointList;
    public string puntoInicial;
    public void ObtenerInfoRuta()
    {
        Input.location.Start();
        lastLocation = Input.location.lastData;
        lastLatitude = lastLocation.latitude;
        lastLongitude = lastLocation.longitude;
        currentDateTime = System.DateTime.Now;
        puntoInicial = NearestPointName();

        SendPuntosRuta(lastLatitude, lastLongitude, puntoInicial,puntoFinal,currentDateTime);
        Input.location.Stop();
    }

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        double distance = Mathf.Sqrt(Mathf.Abs((float)((lat2 - lat1) * (lat2 - lat1) + (lon2 - lon1) * (lon2 - lon1))));
        return distance;
    }

    public string NearestPointName()
    {
        GameObject.Find("AppManager").GetComponent<API>().NearbyPointsAPI();
        float[] positions = gpsloc.GetLastPosition();
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
        return nearestPoint.name;
    }

    IEnumerator SendPuntosRuta(float latitud, float longitud,string PuntoInicial, string PuntoFinal, System.DateTime Fecha)
    {
        const string IP = "144.22.42.236";
        const string port = "3000";
        const string baseURI = "http://"+IP+":"+port+"/api/";
        // Crear formulario
        WWWForm form = new WWWForm();
        form.AddField("puntoInicial", PuntoInicial);
        form.AddField("puntoFinal", PuntoFinal);
        form.AddField("longitud", longitud.ToString());
        form.AddField("latitud", latitud.ToString());
        form.AddField("fecha", Fecha.ToString("dddd, dd MMMM yyyy hh:mm tt"));
        //Realizar request
        UnityWebRequest www = UnityWebRequest.Post(baseURI+"puntobuscado/add", form);
        yield return www.SendWebRequest();
        // Resolucion de la request
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error post: "+ www.error);
            mensajeFinal.text = "Error al Mandar Estadisticas";
        }
        else
        {
            mensajeFinal.text = "Estadisticas Enviadas!!";
            Debug.Log("Form upload complete!");
        }
        yield break;
    }
}
