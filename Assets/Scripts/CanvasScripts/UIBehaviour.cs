using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIBehaviour : MonoBehaviour
{
    // Este script tiene cada funcion que utilizaran los botones de las interfaces, tales como cambiar de interfaz, cambiar de menus, etc.

    //Cambia el estado de un canvas UI, de visible a invisible y viceversa
    public void FlipUI(GameObject ui)
    {
        if (ui.activeSelf)
        {
            Debug.Log("Botón clicado");
            ui.SetActive(false);
        }
        else
        {
            Debug.Log("Botón desclicado");
            ui.SetActive(true);
        }
    }
}
