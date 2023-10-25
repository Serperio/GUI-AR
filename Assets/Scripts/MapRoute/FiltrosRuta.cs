using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FiltrosRuta : MonoBehaviour
{
    [SerializeField]
    MapRouteAPI mapRouteAPI;
    [SerializeField]
    GameObject contentBox;
    [SerializeField]
    GameObject puntoPrefab;

    public void LoadRoutePoints()
    {
        foreach(Transform child in contentBox.transform)
        {
            Destroy(child.gameObject);
        }
        List<Puntito> lastRoute = mapRouteAPI.getLastRoute();
        bool skip = true; // para saltarse el primero
        foreach(Puntito puntito in lastRoute)
        {
            if (!skip)
            {
                GameObject punto = Instantiate(puntoPrefab, Vector3.zero, Quaternion.identity);
                punto.GetComponentInChildren<Text>().text = puntito.nombre;
                punto.transform.SetParent(contentBox.transform);
            }
            else
            {
                skip = false;
            }
        }
    }

    public void PuntosProhibidos()
    {
        GameObject[] puntosRuta = GameObject.FindGameObjectsWithTag("FiltroRuta");
        List<string> puntosProhibidos = new List<string>();
        // Quitar puntos de la ruta
        foreach(GameObject punto in puntosRuta)
        {
            if (!punto.GetComponent<Toggle>().isOn)
            {
                puntosProhibidos.Add(punto.GetComponentInChildren<Text>().text);
            }
        }
        // Quitar escaleras de la ruta
        bool escalerasPermitidas = GameObject.FindGameObjectWithTag("FiltroEscalera").GetComponent<Toggle>().isOn;
        mapRouteAPI.setEscalerasPermitidas(escalerasPermitidas);
        mapRouteAPI.LoadPuntosProhibidos(puntosProhibidos);
    }

    public void LimpiarFiltros()
    {
        GameObject.FindGameObjectWithTag("FiltroEscalera").GetComponent<Toggle>().isOn = true;
        GameObject[] puntosRuta = GameObject.FindGameObjectsWithTag("FiltroRuta");
        foreach(GameObject punto in puntosRuta)
        {
            punto.GetComponent<Toggle>().isOn = true;
        }
    }
}
