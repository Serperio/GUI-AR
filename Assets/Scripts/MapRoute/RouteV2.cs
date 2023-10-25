using UnityEngine;
using System.Collections.Generic;

namespace ARLocation.MapboxRoutes
{
    public class RouteV2 : MonoBehaviour
    {
        public class Point
        {
 
            public Location Location;

            public bool IsStep;

            public string Name;

            public string Instruction;
        }


        public string Name;

        public List<Point> Points = new List<Point>() { new Point { }, new Point { } };

        public float SceneViewScale = 1.0f;

        private bool isDirty = true;
        public bool IsDirty
        {
            get => isDirty;
            set { isDirty = value; }
        }

        public void OnValidate()
        {
            isDirty = true;

            if (Points.Count == 0)
            {
                Points.Add(new Point { });
                Points.Add(new Point { });
            }

            if (Points.Count == 1)
            {
                Points.Add(new Point { });
            }

            if (Points.Count > 0)
            {
                Points[0].IsStep = true;
                Points[Points.Count - 1].IsStep = true;
            }
        }

        public List<Waypoint> GetWaypoints()
        {
            var result = new List<Waypoint>();

            if (Points.Count < 2)
            {
                return result;
            }

            result.Add(new Waypoint { location = Points[0].Location, name = Points[0].Name });
            result.Add(new Waypoint { location = Points[Points.Count - 1].Location, name = Points[Points.Count - 1].Name });

            return result;
        }

        public Route ToMapboxRoute()
        {
            var route = new Route { };
            var leg = new Route.RouteLeg();
            leg.steps = new List<Route.Step>();

            route.geometry = new Route.Geometry();
            route.legs = new List<Route.RouteLeg> { leg };
            route.name = Name; ;

            foreach (var p in Points)
            {
                route.geometry.coordinates.Add(p.Location.Clone());

                if (p.IsStep)
                {
                    var step = new Route.Step();
                    step.geometry = new Route.Geometry();
                    step.geometry.coordinates.Add(p.Location.Clone());
                    step.name = p.Name;
                    step.maneuver = new Route.Maneuver();
                    step.maneuver.location = p.Location.Clone();
                    step.maneuver.instruction = p.Instruction;

                    leg.steps.Add(step);
                }
            }

            return route;
        }
    }
}
