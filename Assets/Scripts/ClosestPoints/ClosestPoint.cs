using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClosestPoints : MonoBehaviour
{
    [SerializeField]
    TextMehProGUI closestPointGUI;   
    [SerializeField] 
    API api;

    // Start is called before the first frame update
    void Start(){
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }
    }  
    
    // Update is called once per frame
    void Update()
    {
        if (Input.location.isEnabledByUser)
        {
            LocationInfo locationInfo = Input.location.lastData;
            double latitude = locationInfo.latitude;
            double longitude = locationInfo.longitude;

            Debug.Log("Latitud: " + latitude + ", Longitud: " + longitude);

            StartCoroutine(NearestPoint(latitude, longitude));
        }
    }

      
    private IEnumerator NearestPoint(double latitude, double longitude)
    {   
        yield return new WaitForSeconds(10f);
        Transform nearestPoint = null;
        float shortestDistance = Mathf.Infinity;

        //-------------------------------------------------
        const string IP = "144.22.42.236";
        //const string IP = "localhost";
        port = "3000";
        baseURI = "http://" + IP + ":" + port + "/api/";

        
        WWWForm form = new WWWForm();

        form.AddField("x", latitude);
        form.AddField("y", longitude);


        UnityWebRequest www = UnityWebRequest.Post(baseURI + "points/nearby", form);
        yield return www.SendwebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error post: "+ www.error);
        }
        else
        {           
            //------------------------------------------------- 
            List<string> data = listJson(response);
            
            List<Point> pointsList = new List<Point>();
            foreach(string dato in data){

                Point point = JsonUtility.FromJson<Point>(dato)
       
                if (point.floor == api.pred.prediction.ToString())
                {
                    pointsList.Add(point);
                } 
                
            }
            //----------------------------------------------
        }
        foreach (Point point in pointsList)
        {
            double distance = CalculateDistance(lat, lon, point.x, point.y);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestPoint = point;
            }
        }

        if (closestPoint != null)
        {
            closestPointText.text = "Closest Point: " + closestPoint.Name;
        }

        closestPointGUI.text = closestPoint.Name;
        StartCoroutine(NearestPoint(latitude, longitude));
    }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        double distance = Mathf.Sqrt(Mathf.Abs((float)((lat2 - lat1) * (lat2 - lat1) + (lon2 - lon1) * (lon2 - lon1))));
        return distance;
    }
    }


