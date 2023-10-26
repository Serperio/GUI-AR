using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HistorialBehaviour : MonoBehaviour
{
    [SerializeField]
    TMP_Dropdown tmpDrop;
    [SerializeField]
    TMP_InputField inputField;
    List<string> SugerenciasAPI= new List<string>() { "Labpro", "abc" };
    private void Start()
    {
        tmpDrop.AddOptions(SugerenciasAPI);
    }

    public void makeSearch()
    {
        inputField.SetTextWithoutNotify(tmpDrop.options[tmpDrop.value].text);
    }
    public void OffDropDown()
    {


            tmpDrop.Hide();
    }
    public void OnDropDown()
    {

            tmpDrop.Show();
    }
}
