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
        // Verificar si el dispositivo tiene brújula
        if (!Input.compass.enabled)
        {
            Debug.LogError("El dispositivo no tiene brújula.");
            return;
        }

        // Habilitar el uso de la brújula
        Input.compass.enabled = true;
    }

    void Update()
    {
        // Obtener el ángulo actualizado de la brújula
        float heading = Input.compass.magneticHeading;

        // Ejemplo de uso: imprimir el ángulo de la brújula en la consola
        Debug.Log("Ángulo de la brújula: " + heading);
        texto.text = "Ángulo de la brújula: " + heading;
        aCamara.text = "Ángulo de la camara: " + camara.transform.rotation.eulerAngles.y;
        aFlecha.text = "Ángulo de la flecha " + flecha.transform.rotation.eulerAngles.y;
        flecha.transform.rotation = Quaternion.Euler(rotacionX, -camara.transform.rotation.eulerAngles.y-heading, 0f);

    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            // Pausar el uso de la brújula cuando la aplicación se pausa
            Input.compass.enabled = false;
        }
        else
        {
            // Reanudar el uso de la brújula cuando la aplicación se reanuda
            Input.compass.enabled = true;
        }
    }

    void OnApplicationQuit()
    {
        // Deshabilitar el uso de la brújula al salir de la aplicación
        Input.compass.enabled = false;
    }
}

