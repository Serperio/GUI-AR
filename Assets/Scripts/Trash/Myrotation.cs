using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Android;

public class Myrotation : MonoBehaviour
{
    private bool hasCompass;
    private Quaternion lastCompassRotation;
    [SerializeField]
    TextMeshProUGUI texto;
    [SerializeField]
    GameObject flecha;
    float rotacionX;

    void Start()
    {
        rotacionX = flecha.transform.rotation.eulerAngles.x;
        // Verificar y solicitar permiso de ubicaci�n (solo necesario para Android)
        if (Application.platform == RuntimePlatform.Android)
        {
            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                Permission.RequestUserPermission(Permission.FineLocation);
            }
        }

        // Verificar si el dispositivo tiene una br�jula
        hasCompass = SystemInfo.supportsGyroscope;

        if (!hasCompass)
        {
            Debug.LogError("El dispositivo no tiene una br�jula.");
            return;
        }

        // Habilitar el uso del giroscopio
        Input.gyro.enabled = true;
    }

    void Update()
    {
        // Obtener la rotaci�n del giroscopio
        lastCompassRotation = Input.gyro.attitude;

        // Ejemplo de uso: imprimir la rotaci�n de la br�jula en la consola
        Debug.Log("Rotaci�n de la br�jula: " + lastCompassRotation);
        flecha.transform.rotation = Quaternion.Euler(rotacionX, lastCompassRotation.eulerAngles.y,0f);
        texto.text = "Rotaci�n de la br�jula: " + lastCompassRotation;
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            // Pausar el uso del giroscopio cuando la aplicaci�n se pausa
            Input.gyro.enabled = false;
        }
        else
        {
            // Reanudar el uso del giroscopio cuando la aplicaci�n se reanuda
            Input.gyro.enabled = true;
        }
    }

    void OnApplicationQuit()
    {
        // Deshabilitar el uso del giroscopio al salir de la aplicaci�n
        Input.gyro.enabled = false;
    }
}

