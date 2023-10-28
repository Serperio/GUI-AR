using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAR : MonoBehaviour
{
    [SerializeField]
    GameObject marker;
    [SerializeField]
    MyPositionGPS gps;

    private Vector2 testPosition = new Vector2( -33.0403f, -71.59032f );

    private void Start()
    {
        //Instantiate(marker, new Vector3(0, 0, 10), Quaternion.identity);
    }

    public void createNew()
    {
        float[] lastPosition = gps.GetLastPosition();
        Vector2 origin = new Vector2(lastPosition[0], lastPosition[1]);
        GPSEncoder.SetLocalOrigin(origin);
        Vector3 unityPosition = GPSEncoder.GPSToUCS(testPosition);
        Debug.Log("Unity Position: " + unityPosition.x + "," + unityPosition.y+","+unityPosition.z);
        Instantiate(marker, unityPosition, Quaternion.identity);
    }
}
