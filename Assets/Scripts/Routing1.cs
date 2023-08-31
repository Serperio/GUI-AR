using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ARLocation.MapboxRoutes;

public class Routing1 : MonoBehaviour
{
    [SerializeField]
    MapboxRoute mapRoute;
    [SerializeField]
    CustomRoute labUXToStairs;
    [SerializeField]
    CustomRoute stairsToP218;
    public void toLab()
    {
        mapRoute.LoadCustomRoute(stairsToP218);
    }
    public void toP218()
    {
        mapRoute.LoadCustomRoute(stairsToP218);
    }
}
