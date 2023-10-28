using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private Vector2 testPosition = new Vector2( -33.0403f, -71.59032f );
    private bool startTracking = false;
    private float initialAngle = 0;
    private float[] lastPosition = null;
    const float maxAngleDiference = 30;

    private GameObject lastInstance = null;

    private void Start()
    {
        //Instantiate(marker, new Vector3(0, 0, 10), Quaternion.identity);
        Input.compass.enabled = true;
        Input.location.Start();
        StartCoroutine(InitializeCompass());
    }

    private IEnumerator InitializeCompass()
    {
        yield return new WaitForSeconds(1);
        startTracking |= Input.compass.enabled;
        if (startTracking)
        {
            initialAngle = Input.compass.trueHeading;
        }
        StartCoroutine(updateTarget());
    }

    private IEnumerator updateTarget()
    {
        while (true)
        {
            createNew();
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
        // Convertir punto de destino a coordenadas de unity
        Vector3 unityPosition = GPSEncoder.GPSToUCS(testPosition);
        // Calcular cambio de angulo de la camara desde el angulo de inicio de la aplicacion
        float newAngle = Input.compass.trueHeading;
        float angleDifference = newAngle - initialAngle;
        Vector3 newPosition =  Vector3.zero;
        // Variable para saber si se debe corregir el angulo de la camara
        bool useNewPosition = false;
        // Revisar si la direccion de la camara es correcta o no
        if (Mathf.Abs(newAngle - aproximateAngleFromCamera()) >= maxAngleDiference)
        {
            useNewPosition = true;
            // Rotar coordenas segun diferencia de angulo
            newPosition = new Vector3(unityPosition.x * Mathf.Cos(angleDifference * Mathf.PI / 180) - unityPosition.z* Mathf.Sin(angleDifference * Mathf.PI / 180),unityPosition.y, unityPosition.x * Mathf.Sin(angleDifference * Mathf.PI / 180) + unityPosition.z * Mathf.Cos(angleDifference * Mathf.PI / 180));
        }
        // Instanciar capsula en al posicion correspondiente
        if (useNewPosition)
        {
            lastInstance = Instantiate(marker, newPosition, Quaternion.identity);
        }
        else
        {
            lastInstance =  Instantiate(marker, unityPosition, Quaternion.identity);
        }
        // Linea para apuntar a la posicion que se debe ir
        guideLine.GetComponent<LineRenderer>().SetPosition(0, arCameraTransform.position);
        guideLine.GetComponent<LineRenderer>().SetPosition(1, lastInstance.transform.position);
        // Mensajes de debug
        Debug.Log("Unity Position: " + unityPosition.x + "," + unityPosition.y+","+unityPosition.z+" ("+Input.compass.trueHeading+","+initialAngle+")");
        Debug.Log("Unity New Position: " + newPosition.x + "," + newPosition.y + "," + newPosition.z);
        Debug.Log("Unity Camera Position: " + arCameraTransform.position.x + "," + arCameraTransform.position.y + "," + arCameraTransform.position.z + "("+arCameraTransform.rotation.eulerAngles+")");
        Debug.Log("Unity Using New Position: " + useNewPosition);
    }

    float aproximateAngleFromCamera()
    {
        return (initialAngle + arCameraTransform.rotation.eulerAngles.y) % 360;
    }
}

