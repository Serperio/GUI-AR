using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ClosestPoint : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI closestPointGUI;   
    [SerializeField] 
    API api;
    [SerializeField]
    MyPositionGPS gpsloc;
    List<Point> pointList1;
    List<Point> pointList;

    // Start is called before the first frame update
    void Start(){
        StartCoroutine(NearestPoint());
    }
      
    private IEnumerator NearestPoint()
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

        pointList = new List<Point>();


        foreach (Point point in pointList1){      
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
        //closestPointGUI.text = "Entro+2";
        if (nearestPoint != null)
        {
            closestPointGUI.text = "En: " + nearestPoint.name;
        }
        else
        {
            //closestPointGUI.text = "No se conoce la ubicación";
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


