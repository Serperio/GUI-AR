using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CompassManager : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI texto;
    [SerializeField]
    TextMeshProUGUI aCamara;
    [SerializeField]
    TextMeshProUGUI aFlecha;
    [SerializeField]
    GameObject flecha;
    [SerializeField]
    GameObject camara;
    float rotacionX;
    float correccion;
    float angleDif;
    void Start()
    {
        float angleDif= Input.compass.magneticHeading;
        rotacionX = flecha.transform.rotation.eulerAngles.x;
        // Verificar si el dispositivo tiene br�jula
        if (!Input.compass.enabled)
        {
            Debug.LogError("El dispositivo no tiene br�jula.");
            return;
        }

        // Habilitar el uso de la br�jula
        Input.compass.enabled = true;
    }

    void Update()
    {
        // Obtener el �ngulo actualizado de la br�jula
        float heading = Input.compass.magneticHeading;

        // Ejemplo de uso: imprimir el �ngulo de la br�jula en la consola
        Debug.Log("�ngulo de la br�jula: " + heading);
        texto.text = "�ngulo de la br�jula: " + heading;
        aCamara.text = "�ngulo de la camara: " + camara.transform.rotation.eulerAngles.y;
        aFlecha.text = "�ngulo de la flecha " + flecha.transform.rotation.eulerAngles.y;
        flecha.transform.rotation = Quaternion.Euler(rotacionX, -camara.transform.rotation.eulerAngles.y-heading, 0f);

    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            // Pausar el uso de la br�jula cuando la aplicaci�n se pausa
            Input.compass.enabled = false;
        }
        else
        {
            // Reanudar el uso de la br�jula cuando la aplicaci�n se reanuda
            Input.compass.enabled = true;
        }
    }

    void OnApplicationQuit()
    {
        // Deshabilitar el uso de la br�jula al salir de la aplicaci�n
        Input.compass.enabled = false;
    }
}

