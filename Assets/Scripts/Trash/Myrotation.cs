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
        // Verificar y solicitar permiso de ubicación (solo necesario para Android)
        if (Application.platform == RuntimePlatform.Android)
        {
            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                Permission.RequestUserPermission(Permission.FineLocation);
            }
        }

        // Verificar si el dispositivo tiene una brújula
        hasCompass = SystemInfo.supportsGyroscope;

        if (!hasCompass)
        {
            Debug.LogError("El dispositivo no tiene una brújula.");
            return;
        }

        // Habilitar el uso del giroscopio
        Input.gyro.enabled = true;
    }

    void Update()
    {
        // Obtener la rotación del giroscopio
        lastCompassRotation = Input.gyro.attitude;

        // Ejemplo de uso: imprimir la rotación de la brújula en la consola
        Debug.Log("Rotación de la brújula: " + lastCompassRotation);
        flecha.transform.rotation = Quaternion.Euler(rotacionX, lastCompassRotation.eulerAngles.y,0f);
        texto.text = "Rotación de la brújula: " + lastCompassRotation;
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            // Pausar el uso del giroscopio cuando la aplicación se pausa
            Input.gyro.enabled = false;
        }
        else
        {
            // Reanudar el uso del giroscopio cuando la aplicación se reanuda
            Input.gyro.enabled = true;
        }
    }

    void OnApplicationQuit()
    {
        // Deshabilitar el uso del giroscopio al salir de la aplicación
        Input.gyro.enabled = false;
    }
}

