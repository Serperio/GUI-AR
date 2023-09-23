using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

public class GameObjectEvento : MonoBehaviour
{
    TextMeshProUGUI nombre;
    TextMeshProUGUI descripcion;
    Image imagen;

    public GameObject panelDescripcion;
    public Evento evento;

    private bool descripcionVisible = false;

    public void ToggleDescripcion()
    {
        descripcionVisible = !descripcionVisible;
        panelDescripcion.SetActive(descripcionVisible);

        if (descripcionVisible)
        {
            // Aquí asigna el texto de descripción al objeto de texto
        
            nombre = panelDescripcion.AddComponent<TextMeshProUGUI>();
            descripcion = panelDescripcion.AddComponent<TextMeshProUGUI>();

            nombre.text = evento.nombre;
            descripcion.text = evento.descripcion;
        }
    }
}
