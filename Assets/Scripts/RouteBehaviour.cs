using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ARLocation.MapboxRoutes
{
    public class RouteBehaviour : MonoBehaviour
    {
        [SerializeField]
        GameObject route;
        [SerializeField]
        GameObject MapBox;
        // Start is called before the first frame update
        public void routeOn()
        {
            MapBox.SetActive(true);
            //MapBox.GetComponent<MapboxRoute>().LoadCustomRoute(route.GetComponent<CustomRoute>());
        }

    }
}
