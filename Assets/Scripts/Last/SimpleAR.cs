using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.XR.ARCoreExtensions;
using UnityEngine.XR.ARFoundation;
using System;

public class SimpleAR : MonoBehaviour
{
    [SerializeField]
    GameObject marker;
    [SerializeField]
    MyPositionGPS gps;
    [SerializeField]
    Transform arCameraTransform;
    [SerializeField]
    GameObject guideLine;
    [SerializeField]
    ClosestPoint closestPoint;

    private Vector2 testPosition = new Vector2( -33.0403f, -71.59032f );
    private bool startTracking = false;
    private Compass initialAngle;
    private float[] lastPosition = null;
    const float maxAngleDiference = 30;

    private GameObject lastInstance = null;

    // new Puntito("2", -33.04031, -71.59031, "pieza") 
    private List<Puntito> route = new List<Puntito>() { new Puntito("1", -33.04036, -71.59033, "patio"), new Puntito("1", -33.04036, -71.59033, "patio")};

    private void Start()
    {
        //Instantiate(marker, new Vector3(0, 0, 10), Quaternion.identity);
        Input.compass.enabled = true;
        //Input.location.Start();
        StartCoroutine(InitializeCompass());
    }

    private IEnumerator InitializeCompass()
    {
        yield return new WaitForSeconds(1);
        startTracking |= Input.compass.enabled;
        if (startTracking)
        {
            initialAngle = Input.compass;
        }
        StartCoroutine(updateTarget());
    }

    private IEnumerator updateTarget()
    {
        while (true)
        {
            createNew();
            Debug.Log($"aaaaaaaaaaaaaaaaaaaaaaaaaa: {initialAngle.headingAccuracy}");
            yield return new WaitForSeconds(1);
        }
    }

    public void createNew()
    {
        // Eliminar ultima capsula colocada
        if(lastInstance != null)
        {
            Destroy(lastInstance);
        }
        // Setear punto de partida para conversion de unidades
        if (lastPosition == null)
        {
            lastPosition = gps.GetLastPosition();
            Vector2 origin = new Vector2(lastPosition[0], lastPosition[1]);
            GPSEncoder.SetLocalOrigin(origin);
        }
        // Hacer ruta mientras queden puntos por recorrer
        if(route.Count > 1)
        {
            // Quitar punto al que se llego
            if(closestPoint.lastPosition != null && closestPoint.lastPosition.name == route[1].nombre)
            {
                route.RemoveAt(1);
            }
            // Convertir punto de destino a coordenadas de unity
            Vector3 unityPosition = GPSEncoder.GPSToUCS(puntitoToVector2(route[1]));
            // Calcular cambio de angulo de la camara desde el angulo de inicio de la aplicacion
            float newAngle = Input.compass.trueHeading;
            //float angleDifference = newAngle - initialAngle;
            Vector3 newPosition =  Vector3.zero;
            // Rotacion que se debe hacer para apuntar al norte
            //float rotationToNorth = initialAngle;
            // Variable para saber si se debe corregir el angulo de la camara
            bool useNewPosition = false;
            // Revisar si la direccion de la camara es correcta o no
            //if (Mathf.Abs(newAngle - aproximateAngleFromCamera()) >= maxAngleDiference)
            //{
                //useNewPosition = true;
                // Rotar coordenas segun diferencia de angulo
                //newPosition = new Vector3(unityPosition.x * Mathf.Cos(angleDifference * Mathf.PI / 180) - unityPosition.z* Mathf.Sin(angleDifference * Mathf.PI / 180),unityPosition.y, unityPosition.x * Mathf.Sin(angleDifference * Mathf.PI / 180) + unityPosition.z * Mathf.Cos(angleDifference * Mathf.PI / 180));
            //}
            // Instanciar capsula en al posicion correspondiente
            if (useNewPosition)
            {
                lastInstance = Instantiate(marker, newPosition, Quaternion.identity);
                lastInstance.AddComponent<ARAnchor>();
            }
            else
            {
                lastInstance =  Instantiate(marker, unityPosition, Quaternion.identity);
                lastInstance.AddComponent<ARAnchor>();
            }
            // Linea para apuntar a la posicion que se debe ir
            guideLine.GetComponent<LineRenderer>().SetPosition(0, arCameraTransform.position);
            guideLine.GetComponent<LineRenderer>().SetPosition(1, lastInstance.transform.position);
            // Mensajes de debug
            Debug.Log("Unity Position: " + unityPosition.x + "," + unityPosition.y+","+unityPosition.z+" ("+Input.compass.trueHeading+","+initialAngle+")");
            Debug.Log("Unity New Position: " + newPosition.x + "," + newPosition.y + "," + newPosition.z);
            Debug.Log("Unity Camera Position: " + arCameraTransform.position.x + "," + arCameraTransform.position.y + "," + arCameraTransform.position.z + "("+arCameraTransform.rotation.eulerAngles+")");
            Debug.Log("Unity Using New Position: " + useNewPosition);
        } else
        {
            Debug.Log("Unity Ruta completa");
        }
    }
    // Cargar la ruta a seguir
    public void LoadRoute(List<Puntito> customRoute)
    {
        route = customRoute;
        StartCoroutine(updateTarget());
    }
    // Conversion de formato punto a vector <latitud, longitud>
    Vector2 puntitoToVector2(Puntito p)
    {
        return new Vector2((float)p.latitud, (float)p.longitud);
    }
    // Angulo al que deberia estar mirando la camara
    //float aproximateAngleFromCamera()
    //{
        //return (initialAngle + arCameraTransform.rotation.eulerAngles.y) % 360;
    //}
}

