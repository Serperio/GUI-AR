using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ARLocation.MapboxRoutes
{
    public class NewBehaviourScript : MonoBehaviour
    {
        public CustomRoute customRoute;
        //GameObject prefabRoute;
        //[SerializeField]
        //MapboxRoute mbox;

        // Start is called before the first frame update
        void Awake()
        {

            customRoute.modifyPoint(0, "A", -33.03503f, -71.5968f);
            customRoute.modifyPoint(1, "B", -33.03503f, -71.5968f);
            customRoute.makePoint("B", -33.03503f, -71.5968f);
            //prefabRoute = GameObject.Find("PrefabRoute");
            //prefabRoute.GetComponent<RouteBehaviour>().routeOn();
            //mbox.LoadCustomRoute(customRoute);

        }  

        // Update is called once per frame
        void Update()
        {

        }
    }
}
