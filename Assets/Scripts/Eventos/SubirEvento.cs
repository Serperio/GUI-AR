using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
public class SubirEvento : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    TMP_InputField InputNombre;

    [SerializeField]
    TMP_InputField InputDescripcion;

    //System.DateTime fecha = System.DateTime.Now;

    public void CheckEvento(){
        System.DateTime fecha = System.DateTime.Now;
        Debug.Log("nombre: "+InputNombre.text+"\ndescripcion: "+InputDescripcion.text+"\nfecha: "+fecha);
    }

    IEnumerator SendEvento(string nombre, string descripcion)
    {
        //const string IP = "144.22.42.236";
        System.DateTime fecha = System.DateTime.Now;
        Debug.Log("nombre: "+nombre+"\ndescripcion: "+descripcion+"\nfecha: "+fecha.ToString());
        const string IP = "144.22.42.236";
        const string port = "3000";
        const string baseURI = "http://"+IP+":"+port+"/api/";
        // Crear formulario
        WWWForm form = new WWWForm();
        form.AddField("nombre", nombre);
        form.AddField("descripcion", descripcion);
        form.AddField("fecha", fecha.ToString());
        //form.AddField("imagen", );
        //Realizar request
        UnityWebRequest www = UnityWebRequest.Post(baseURI+"evento/add", form);
        yield return www.SendWebRequest();
        // Resolucion de la request
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error post: "+ www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
        }
        yield break;
    }

    public void GeneraEvento()
    {
        string nombre = InputNombre.text;
        string descripcion = InputDescripcion.text;
        StartCoroutine(SendEvento(nombre, descripcion));
    }

}
