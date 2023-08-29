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
        api.NearbyPointsAPI();
        float[] positions = gpsloc.GetLastPosition();
        float latitude = positions[0]; //Pasar
        float longitude = positions[1]; //Pasar de gps
        yield return new WaitForSeconds(10f);
        Point nearestPoint = null;
        double shortestDistance = Mathf.Infinity;
        pointList1 = api._pointlist; 
        closestPointGUI.text=pointList1.Count.ToString();


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
    }


