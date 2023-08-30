using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ARLocation.MapboxRoutes;

public class Routing1 : MonoBehaviour
{
    GameObject rutaEscalera;
    GameObject Labux;
    [SerializeField]
    MapboxRoute mapRoute;
    [SerializeField]
    CustomRoute cus;
    public void toLab()
    {
        mapRoute.LoadCustomRoute(cus);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
