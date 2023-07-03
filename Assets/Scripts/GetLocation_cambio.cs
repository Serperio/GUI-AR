using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public class GetLocation_cambio : MonoBehaviour
{
    struct Geo
    {
        public float latitud;
        public float longitud;
    }

    [SerializeField]
    TextMeshProUGUI texto;
    [SerializeField]
    TextMeshProUGUI puntoASeguir;
    [SerializeField]
    TextMeshProUGUI puntoCercano;
    [SerializeField]
    TextMeshProUGUI coordenadasTransformadas;
    [SerializeField]
    GameObject monito;
    [SerializeField]
    GameObject monito2;



    Vector3 posicionActual;
    bool instanciado = false;
    bool guardarPunto = false;
    List<Geo> puntos = new List<Geo>();
    Geo lastLocation = new Geo();
    private void Start()
    {


        StartCoroutine(ola());
        lastLocation.latitud = -1;
        lastLocation.longitud = -1;
    }

  /*  void Update()
    {
        GetUserLocation();
    }*/

    public Vector3 GetUserLocation()
    {
        if (!Input.location.isEnabledByUser) //FIRST IM CHACKING FOR PERMISSION IF "true" IT MEANS USER GAVED PERMISSION FOR USING LOCATION INFORMATION
        {
            texto.text = "No Permission";
            Permission.RequestUserPermission(Permission.FineLocation);
        }
        else
        {
            texto.text = "Permission Granted";
            StartCoroutine(ola());
        }
        return posicionActual;
    }

    IEnumerator ola()
    {
        //texto.text="1";
        // Check if the user has location service enabled.
#if UNITY_EDITOR
                //Wait until Unity connects to the Unity Remote, while not connected, yield return null
        while (!UnityEditor.EditorApplication.isRemoteConnected)
        {
            yield return null;
        }
#endif

        if (!Input.location.isEnabledByUser)
        {
            texto.text = "no funco";
            yield break;
        }
        // Starts the location service.
        Input.location.Start();
        texto.text = "Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude;
        if (guardarPunto)
        {
            //Instantiate(prefab, Vector3.zero, Quaternion.identity);
            lastLocation.latitud = Input.location.lastData.latitude;
            lastLocation.longitud = Input.location.lastData.longitude;
            puntos.Add(lastLocation);
            guardarPunto = false;
            //instanciado = true;
        }
        else
        {
            string variable = "";
            Geo actual = new Geo();

            //Posicion Actual
            actual.latitud = Input.location.lastData.latitude;
            actual.longitud = Input.location.lastData.longitude;

            //Punto mas cercano
            Geo minDistP = new Geo();
            minDistP.latitud = -10000000;
            minDistP.longitud = -1000000000000;
            float minDist = 100000;

            //Se calcula el punto mas cercano
            foreach (Geo point in puntos)
            {
                float dist = Mathf.Abs(point.latitud - actual.latitud) + Mathf.Abs(point.longitud - actual.longitud);
                if (dist < minDist)
                {
                    minDistP.latitud = point.latitud;
                    minDistP.longitud = point.longitud;
                    minDist = dist;
                }
                variable += point.latitud + ", " + point.longitud + "\n";
            }

            puntoASeguir.text = variable;
            puntoCercano.text = minDistP.latitud + ", " + minDistP.longitud + "\n";
            float difLatitud = minDistP.latitud - actual.latitud;
            float difLongitud = minDistP.longitud - actual.longitud;
            if (puntos.Count > 0)
            {
                //Si cambio el Actual a MinDisp, el monito tendra la posicion del punto mas cercano
                Vector3 xyz_vector = Quaternion.AngleAxis(actual.longitud, -Vector3.up) * Quaternion.AngleAxis(actual.latitud, -Vector3.right) * new Vector3(0, 0, 1);
                coordenadasTransformadas.text = "x:" + xyz_vector.x + "\ny:" + xyz_vector.y + "\nz:" + xyz_vector.z;
                //Recoloca el monito en la posicion actual
                monito.transform.position = xyz_vector;
            }
        }
        // Waits until the location service initializes
        /*
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }
        
        // If the service didn't initialize in 20 seconds this cancels location service use.
        if (maxWait < 1)
        {
            print("Timed out");
            yield break;
        }

        // If the connection failed this cancels location service use.
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            print("Unable to determine device location");
            yield break;
        }
        else
        {
            posicionActual = new Vector3(Input.location.lastData.latitude, Input.location.lastData.longitude,0);
            texto.text = "Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude;
            // If the connection succeeded, this retrieves the device's current location and displays it in the Console window.
            //print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
        }*/

        // Stops the location service if there is no need to query location updates continuously.
        Input.location.Stop();
        yield return new WaitForSeconds(3);
        StartCoroutine(ola());

    }
    public void GuardarLocation()
    {
        //Agrega el punto la lista e.d longitud latitud en pantalla
        /*lastLocation.latitud = Input.location.lastData.latitude;
        lastLocation.longitud = Input.location.lastData.longitude;
        puntos.Add(lastLocation);


        //Posicion Actual*/
        Input.location.Start();
        Geo actual2 = new Geo();
        actual2.latitud = Input.location.lastData.latitude;
        actual2.longitud = Input.location.lastData.longitude;
        Vector3 xyz_vector_aux = Quaternion.AngleAxis(actual2.longitud, -Vector3.up) * Quaternion.AngleAxis(actual2.latitud, -Vector3.right) * new Vector3(0, 0, 1);
        monito2.transform.position = xyz_vector_aux;
        Input.location.Stop();
    }
}