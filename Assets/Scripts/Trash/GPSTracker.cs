using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPSTracker : MonoBehaviour
{
    [SerializeField]
    GameObject monito;
   
    // Let's say our origin, the point (0, 0) in Unity, corresponds to these GPS coordinates
    public float originLatitude = -33.03474f;
    public float originLongitude = -71.59647f;

    // Our scale factors
    public float scaleLatitude = 111f;  // Roughly 111km per degree of latitude
    public float scaleLongitude = 111f; // This is only true at the equator. At other latitudes, multiply this by Cos(latitude)

    // The position in Unity
    public Vector3 UnityPosition;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartLocationService());
    }

    private IEnumerator StartLocationService()
    {
        // Check for location service
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("User has not enabled GPS");
            yield break;
        }

        // Start service before querying location
        Input.location.Start();

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            Debug.Log("Timed out");
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Unable to determine device location");
            yield break;
        }
        else
        {
            // Access granted and location value could be retrieved
            float latitude = Input.location.lastData.latitude;
            float longitude = Input.location.lastData.longitude;

            // Calculate position in Unity space
            float x = (longitude - originLongitude) * scaleLongitude;
            float z = (latitude - originLatitude) * scaleLatitude;
            UnityPosition = new Vector3(x, 0, z);
            monito.transform.position = UnityPosition;
        }

        // Stop service if there is no need to query location updates continuously
        Input.location.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Unity position: " + UnityPosition);
    }
}