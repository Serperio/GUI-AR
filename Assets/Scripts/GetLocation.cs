using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class GetLocation : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI texto;
    Vector3 posicionActual;
    private void Start()
    {
        StartCoroutine(ola());
    }

    void Update()
    {
        GetUserLocation();
    }

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
        texto.text = "Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp;
        // Waits until the location service initializes
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
            texto.text = "Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp;
            // If the connection succeeded, this retrieves the device's current location and displays it in the Console window.
            //print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
        }

        // Stops the location service if there is no need to query location updates continuously.
        Input.location.Stop();
    }
}