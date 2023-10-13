using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARLocation.MapboxRoutes
{
    public class RutaCustomAPI : MonoBehaviour
    {

        public MapboxRoute mapbox;

        public void LoadRoute(List<Puntito> puntos)
        {
            mapbox.GetComponent<MapboxRoute>().LoadCustomRoute(getRoute(puntos));
        }

        public static RouteV2 getRoute(List<Puntito> puntos)
        {
            RouteV2 route = new RouteV2();
            // Agregar el primero
            route.Points[0].Location = new Location(puntos[0].latitud, puntos[0].longitud, 0f);
            route.Points[0].Name = puntos[0].nombre;
            route.Points[0].Instruction = puntos[0].nombre;
            route.Points[0].IsStep = true;

            for (int i = 1; i < puntos.Count - 1; i++) // Agregar desde el segundo al penultimo
            {
                route.Points.Add(new RouteV2.Point());
                route.Points[i].Location = new Location(puntos[i].latitud, puntos[i].longitud, 0f);
                route.Points[i].Name = puntos[i].nombre;
                route.Points[i].Instruction = puntos[i].nombre;
                route.Points[i].IsStep = true;

            }

            // Agregar el ultimo
            route.Points[puntos.Count - 1].Location = new Location(puntos[puntos.Count - 1].latitud, puntos[puntos.Count - 1].longitud, 0f);
            route.Points[puntos.Count - 1].Name = puntos[puntos.Count - 1].nombre;
            route.Points[puntos.Count - 1].Instruction = puntos[puntos.Count - 1].nombre;
            route.Points[puntos.Count - 1].IsStep = true;

            //route.Points.Add(new RouteV2.Point());

            /*
            route.Points[0].Location = new Location(-33.03481, -71.59653, 0f);
            route.Points[0].Name = "inicio";
            route.Points[0].Instruction = "inicio";
            route.Points[0].IsStep = true;

            route.Points[1].Location = new Location(-33.03484, -71.59666, 0f);
            route.Points[1].Name = "escalera";
            route.Points[1].Instruction = "escalera";
            route.Points[1].IsStep = true;
            */
            /*
            route.Points[2].Location = new Location(-33.03505, -71.59661, 0f);
            route.Points[2].Name = "fin";
            route.Points[2].Instruction = "fin";
            route.Points[2].IsStep = true;*/
            return route;
        }
    }

}
