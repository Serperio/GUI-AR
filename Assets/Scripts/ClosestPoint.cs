using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Globalization;

public class ClosestPoint : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI closestPointGUI;
    [SerializeField]
    TextMeshProUGUI closestPointInfo;
    [SerializeField]
    API api;
    [SerializeField]
    MyPositionGPS gpsloc;
    [SerializeField]
    TextMeshProUGUI mensajeFinal;
    [SerializeField]
    GameObject panelErrorDB;
    List<Point> pointList1;
    List<Point> pointList;
    private System.DateTime fecha;

    // Start is called before the first frame update
    void Start()
    {
        SubirDummyInteres();
        StartCoroutine(NearestPoint());
    }
    
    public void SubirDummyInteres()
    {
        StartCoroutine(EnviarDataPuntoInteres("dummy1", (float)-33.03518, (float)-71.59681));
        EnviarDataPuntoInteres("dummy2", (float)-33.03481, (float)-71.59657);
        EnviarDataPuntoInteres("dummy3", (float)-33.036278, (float)-71.5957897);
    }

    public IEnumerator EnviarDataPuntoInteres(string name, float latitud, float longitud)
    {
        //const string IP = "144.22.42.236";
        //Debug.Log("Inicio: "+PuntoInicial+"\nfinal: "+PuntoFinal+"\Fecha+hora: "+Fecha);
        const string IP = "144.22.42.236";
        const string port = "3000";
        const string baseURI = "http://"+IP+":"+port+"/api/";
        // Crear formulario
        fecha = System.DateTime.Now;
        print("ENVOLA FUNCIONAAA SIONO");
        print("fecha es: "+fecha);
        WWWForm form = new WWWForm();
        form.AddField("nombre", name);
        form.AddField("longitud", longitud.ToString(CultureInfo.InvariantCulture));
        form.AddField("latitud", latitud.ToString(CultureInfo.InvariantCulture));
        form.AddField("fecha", fecha.ToString("dddd, dd MMMM yyyy hh:mm tt"));
        //Realizar request
        UnityWebRequest www = UnityWebRequest.Post(baseURI+"puntosinteres/add", form);
        yield return www.SendWebRequest();
        // Resolucion de la request
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error post: "+ www.error);
            Debug.Log("NO SE PUDO SUBIR INTERESSSSSSSSSSSSSSSSSS");
            mensajeFinal.text = "Error al Mandar Estadisticas\nBD no disponible";
            panelErrorDB.SetActive(true);
        }
        else
        {
            //mensajeFinal.text = "Estadisticas Enviadas!!";
            Debug.Log("Form upload complete!");
        }
        yield break;
    }

    public IEnumerator NearestPoint()
    {
        yield return new WaitForSeconds(10f);
        GameObject.Find("AppManager").GetComponent<API>().NearbyPointsAPI();
        float[] positions = gpsloc.GetLastPosition();
        float latitude = positions[0]; //Pasar
        float longitude = positions[1]; //Pasar de gps
        Point nearestPoint = null;
        double shortestDistance = Mathf.Infinity;
        pointList1 = api._pointlist;
        //closestPointGUI.text= api._pointlist.Count.ToString();
        closestPointGUI.text = "Cargando...";
        closestPointInfo.text = "Cargando...";
        Debug.Log("ENTRAMOS A NEARESTPOINT");
        //SubirDummyInteres();
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
        if (nearestPoint != null)
        {
            closestPointGUI.text = "En: " + nearestPoint.name;
            closestPointInfo.text = nearestPoint.name;
            if(nearestPoint.tipo == "Ruta")
            {
                EnviarDataPuntoInteres(nearestPoint.name,latitude,longitude);
            }
        }
        else
        {
            closestPointGUI.text = "No se conoce la ubicaci�n";
            closestPointInfo.text = "No se conoce la ubicaci�n";
        }
        StartCoroutine(NearestPoint());
    }

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        double distance = Mathf.Sqrt(Mathf.Abs((float)((lat2 - lat1) * (lat2 - lat1) + (lon2 - lon1) * (lon2 - lon1))));
        return distance;
    }

    private void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, message, 0);
                toastObject.Call("show");
            }));
        }
    }
}