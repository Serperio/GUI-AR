using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace ARLocation.MapboxRoutes
{
    public class RoutetoAR : MonoBehaviour
    {
        bool moreTimes = true;
        [SerializeField]
        CustomRoute customRoute;
        GameObject prefabRoute;
        [SerializeField]
        GameObject newRoute;
        //[SerializeField]
        //TextMeshProUGUI tmprougui;
        //[SerializeField]
        //MapboxRoute mbox;

        List<Point> points= new List<Point>();
        // Start is called before the first frame update
        //void Awake()
        //{
        //    points.Add(new Point(-33.03503f, -71.5968f, "A"));
         //   points.Add(new Point(-33.03503f, -71.5968f, "A"));
          //  points.Add(new Point(-33.03503f, -71.5968f, "A"));
           // initiateRoute(points);
            /*customRoute.modifyPoint(0, "A", -33.03503f, -71.5968f);
            customRoute.modifyPoint(1, "B", -33.03503f, -71.5968f);
            customRoute.makePoint("B", -33.03503f, -71.5968f);
            prefabRoute = GameObject.Find("PrefabRoute");
            prefabRoute.GetComponent<RouteBehaviour>().routeOn();*/
            //mbox.LoadCustomRoute(customRoute);

        //}  

        // Update is called once per frame
        public void initiateRoute(List<Point> pointList)
        {

            Instantiate(newRoute);
            prefabRoute = GameObject.Find("PrefabRoute(Clone)");
            customRoute = prefabRoute.GetComponentInChildren<CustomRoute>();
            int count = 0;
            foreach (Point point in pointList)
            {
                if (count >= 2)
                {
                    //customRoute.makePoint(point.name, point.x, point.y);
                }
                else
                {
                    //customRoute.modifyPoint(count, point.name, point.x, point.y);
                }
                count++;
            }
            //tmprougui.text = customRoute.Points.Count.ToString();
            prefabRoute.GetComponent<RouteBehaviour>().routeOn();
            /*if (!moreTimes)
            {
                moreTimes = true;
            }
            else
            {
                delRoute();
            }*/

        }
        void delRoute()
        {
            Destroy(prefabRoute);
        }
    }
}
