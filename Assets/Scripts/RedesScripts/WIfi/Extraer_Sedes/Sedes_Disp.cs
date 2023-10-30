using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

[System.Serializable]

public class Sedes_Disp : MonoBehaviour
{
    public TMP_Dropdown dropdownSede;
    private List<string> names = new List<string>();

    private void Start()
    {
        dropdownSede.captionText.text = "Seleccione una opción";
        StartCoroutine(SedesDisponibles());
        // Escucha el evento de cambio de selecci�n de los Dropdowns.
        //dropdownPiso.onValueChanged.AddListener(delegate { OnDropdownValueChanged(dropdownPiso, ref selectedValuePiso); });
        //dropdownEdificio.onValueChanged.AddListener(delegate { OnDropdownValueChanged(dropdownEdificio, ref selectedValueEdificio); });
    }

    static List<string> JsonToList(string jsonData)
    { //Convierte Json a Lista
        string json = jsonData.Substring(1, jsonData.Length - 2);
        bool startParentesis = false;
        bool endParentesis = false;
        string dato = "";
        List<string> strings = new List<string>();
        foreach (char caracter in json)
        {
            if (caracter == '{')
            {
                dato = "";
                startParentesis = true;
            }
            else if (caracter == '}')
            {
                endParentesis = true;
            }
            if (startParentesis)
            {
                dato += caracter;
            }
            if (endParentesis)
            {
                startParentesis = false;
                endParentesis = false;
                strings.Add(dato);
            }
        }
        return strings;
    }

    IEnumerator SedesDisponibles()
    {
        const string IP = "144.22.42.236";
        //const string IP = "localhost";
        const string port = "3000";
        const string baseURI = "http://" + IP + ":" + port + "/api/";

        UnityWebRequest www = UnityWebRequest.Get(baseURI + "sedes");
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error post: " + www.error);
        }
        else
        {
            // Recuperar JSON
            string response = www.downloadHandler.text;
            // Obtener listado de puntos
            List<string> data = JsonToList(response);
            // Transformar JSON a Point
            List<Sede> sede = new List<Sede>();
            foreach (string dato in data)
            {
                Sede cada_sede = JsonUtility.FromJson<Sede>(dato);
                sede.Add(cada_sede);
            }
            // Entregar resultados
            if (sede.Count > 0)
            { 
                foreach (Sede cada_sede in sede)
                {
                    names.Add(cada_sede.name);
                    //GameObject texto = Instantiate(item, Vector3.zero, Quaternion.identity);
                    //texto.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = cada_sede.name;
                    //texto.transform.parent = contenido.transform;
                    //texto.transform.localPosition = Vector3.zero;
                    //texto.transform.localScale = Vector3.one;
                    //Debug.Log(point.tipo);
                }
                dropdownSede.ClearOptions();
                dropdownSede.AddOptions(names);
            }

        }
    }


    // Llama a esta función para llenar el Dropdown con una lista de nombres

   // public void OnDropdownValueChanged(int index)
    //{
     //   string selectedName = dropdownSede.options[index].text;
      //  label.text =  selectedName;
    //}
}