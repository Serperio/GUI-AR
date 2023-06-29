using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIBehaviour : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI csm;
    [SerializeField]
    TextMeshProUGUI wifiOn;
    [SerializeField]
    TextMeshProUGUI Datos;
    [SerializeField]
    Canvas canvasBueno;
    [SerializeField]
    Canvas canvasMalo;
    private void Update()
    {
        /*
        if(csm.text== "Si se ha activado el GPS" && (Datos.text=="Si hay datos"   || wifiOn.text == "Si se ha activado el Wifi"))
        {
            canvasBueno.enabled = true;
            canvasMalo.enabled = false;
        }
        else
        {
            canvasBueno.enabled = false;
            canvasMalo.enabled = true;
        }*/
    }
    // Start is called before the first frame update
    public void FlipUI(GameObject ui)
    {
        if (ui.activeSelf)
        {
            ui.SetActive(false);
        }
        else
        {
            ui.SetActive(true);
        }
    }
    public void FlipPoints(GameObject mp)
    {
        if (mp.GetComponent<Marked_Points>().enabled==true)
        {
            mp.GetComponent<Marked_Points>().enabled = false;
        }
        else
        {
            mp.GetComponent<Marked_Points>().enabled = true;
        }
    }
    public void colorFlip(GameObject mp)
    {
        if (mp.GetComponent<Image>().color == Color.red)
        {
            mp.GetComponent<Image>().color = Color.white;
        }
        else
        {
            mp.GetComponent<Image>().color = Color.red;
        }
    }
}
