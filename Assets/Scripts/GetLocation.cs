using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

public class GetLocation : MonoBehaviour
{
    public GameObject locationPrefab; // The prefab you want to instantiate at the real world location
    public float originLatitude = -33.0472f; // Origin latitude for Valparaiso
    public float originLongitude = -71.6127f; // Origin longitude for Valparaiso
    public float scale = 100000f; // Scale factor, 1 degree latitude is approximately 111km so scale factor should be around 100000 for AR use
    int i = 1;
    bool save = false;
    List<Puntito> ruta = new List<Puntito>();
    float horizontalAccuracy;
    float verticalAccuracy;
    float relativeError;
    [SerializeField]
    TextMeshProUGUI Error;

    private ARSessionOrigin arOrigin;

    void Start()
    {
        arOrigin = GetComponent<ARSessionOrigin>();
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
        Input.location.Start(5,10);

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
            // Sección calculo del error
            horizontalAccuracy = Input.location.lastData.horizontalAccuracy;
            verticalAccuracy = Input.location.lastData.verticalAccuracy;
            //relativeError = Mathf.Sqrt(Mathf.Pow(horizontalAccuracy, 2) + Mathf.Pow(verticalAccuracy, 2));

            //Debug.Log("Relative Error: " + relativeError.ToString());
            Error.text = "Vert. Relative Error: " + verticalAccuracy.ToString() + " [m]\nHoriz. Relative Error: " + horizontalAccuracy.ToString() +  " [m]";

            float latitude = Input.location.lastData.latitude;
            float longitude = Input.location.lastData.longitude;
            Puntito punto_actual = new Puntito(i.ToString(),(double)latitude,(double)longitude);
            ruta.Add(punto_actual);

            // Calculate position in Unity space
            Vector3 position = new Vector3((longitude - originLongitude) * scale, 0, (latitude - originLatitude) * scale);
    
            // Instantiate object at calculated position relative to ARSessionOrigin
            //arOrigin.MakeContentAppearAt(locationPrefab.transform, position,);
            ARAnchor arAnchor = locationPrefab.AddComponent<ARAnchor>();
            arOrigin = FindObjectOfType<ARSessionOrigin>();
            locationPrefab.transform.SetParent(arOrigin.transform);
            //Instantiate(locationPrefab, position, Quaternion.identity, arOrigin.transform);
        }

        // Stop service if there is no need to query location updates continuously
        Input.location.Stop();
    }
}