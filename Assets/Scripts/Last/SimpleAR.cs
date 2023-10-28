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

    private Vector2 testPosition = new Vector2( -33.0403f, -71.59032f );
    private bool startTracking = false;
    private float initialAngle = 0;
    private float[] lastPosition = null;

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
    }

    public void createNew()
    {
        if (lastPosition == null)
        {
            lastPosition = gps.GetLastPosition();
            Vector2 origin = new Vector2(lastPosition[0], lastPosition[1]);
            GPSEncoder.SetLocalOrigin(origin);
        }
        Vector3 unityPosition = GPSEncoder.GPSToUCS(testPosition);
        float newAngle = Input.compass.trueHeading;
        float angleDifference = newAngle - initialAngle;
        Vector3 newPosition = new Vector3(unityPosition.x * Mathf.Cos(initialAngle * Mathf.PI / 180) - unityPosition.z* Mathf.Sin(initialAngle * Mathf.PI / 180),unityPosition.y, unityPosition.x * Mathf.Sin(initialAngle * Mathf.PI / 180) + unityPosition.z * Mathf.Cos(initialAngle * Mathf.PI / 180));
        Debug.Log("Unity Position: " + unityPosition.x + "," + unityPosition.y+","+unityPosition.z+" ("+Input.compass.trueHeading+","+initialAngle+")");
        Debug.Log("Unity New Position: " + newPosition.x + "," + newPosition.y + "," + newPosition.z);
        Debug.Log("Unity Camera Position: " + arCameraTransform.position.x + "," + arCameraTransform.position.y + "," + arCameraTransform.position.z + "("+arCameraTransform.rotation.eulerAngles+")");
        Instantiate(marker, newPosition, Quaternion.identity);
    }
}
