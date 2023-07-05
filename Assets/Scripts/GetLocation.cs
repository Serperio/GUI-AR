using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

public class GetLocation : MonoBehaviour
{
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
        //Input.location.Start(5,10);

        // Wait until service initializes
        // Starts the location service.
        Input.location.Start();
        // Waits until the location service initializes
        
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
            horizontalAccuracy = Input.location.lastData.horizontalAccuracy*0.46f;
            verticalAccuracy = Input.location.lastData.verticalAccuracy*0.46f;
            //relativeError = Mathf.Sqrt(Mathf.Pow(horizontalAccuracy, 2) + Mathf.Pow(verticalAccuracy, 2));

            //Debug.Log("Relative Error: " + relativeError.ToString());
            Error.text = "Vert. Relative Error: " + verticalAccuracy.ToString() + " [m]\nHoriz. Relative Error: " + horizontalAccuracy.ToString() +  " [m]";
        }

        // Stop service if there is no need to query location updates continuously
        Input.location.Stop();
    }
}