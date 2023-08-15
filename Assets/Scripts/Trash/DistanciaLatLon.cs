using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DistanciaLatLon : MonoBehaviour
{
    public static double toRadians(double angulo)
    {
        // Angle in 10th
        // of a degree
        return (angulo * Math.PI) / 180;
    }

    public static double distance(double lat1, double lat2, double lon1, double lon2)
    {
 
        // The math module contains
        // a function named toRadians
        // which converts from degrees
        // to radians.
        lon1 = toRadians(lon1);
        lon2 = toRadians(lon2);
        lat1 = toRadians(lat1);
        lat2 = toRadians(lat2);
 
        // Haversine formula
        double dlon = lon2 - lon1;
        double dlat = lat2 - lat1;
        double a = Math.Pow(Math.Sin(dlat / 2), 2) +
                   Math.Cos(lat1) * Math.Cos(lat2) *
                   Math.Pow(Math.Sin(dlon / 2),2);
             
        double c = 2 * Math.Asin(Math.Sqrt(a));
 
        // Radius of earth in
        // kilometers. Use 3956
        // for miles (MANTENER EN 6371)
        double r = 6371;
 
        // calculate the result
        return (c * r);
    }

}
