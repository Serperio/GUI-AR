using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

public class GetLocation : MonoBehaviour
{
    /*
     * Permite mostrar el error
     */
    float horizontalAccuracy;
    float verticalAccuracy;

    [SerializeField]
    TextMeshProUGUI Error;
    void Start()
    {
        StartCoroutine(StartLocationService());
    }

    private IEnumerator StartLocationService()
    {
        Input.location.Start();
        horizontalAccuracy = Input.location.lastData.horizontalAccuracy * 0.46f;
        verticalAccuracy = Input.location.lastData.verticalAccuracy * 0.46f;

        Error.text = "Vert. Relative Error: " + verticalAccuracy.ToString() + " [m]\nHoriz. Relative Error: " + horizontalAccuracy.ToString() + " [m]";
        Input.location.Stop();
        yield return new WaitForSeconds(1);
        StartCoroutine(StartLocationService());
    }
}