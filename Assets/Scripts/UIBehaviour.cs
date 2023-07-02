using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIBehaviour : MonoBehaviour
{
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
