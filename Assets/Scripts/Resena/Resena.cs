using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

public class Resena : MonoBehaviour
{
    [SerializeField]
    public TMP_InputField InputPuntoInicial;
    [SerializeField]
    public TMP_InputField InputPuntoFinal;
    [SerializeField]
    public TMP_InputField InputPuntoFallo;
    [SerializeField]
    public TMP_InputField InputMotivo;

    [SerializeField]
    public TextMeshProUGUI MensajeResena;

    public void GeneraResena()
    {
        MensajeResena.text = "El reporte no ha podido ser enviado, intente nuevamente.";
        string inicio = InputPuntoInicial.text;
        string final = InputPuntoFinal.text;
        string puntoFallo = InputPuntoFallo.text;
        string motivo = InputMotivo.text;
        SendResena(inicio, final, puntoFallo, motivo);
    }

    public void ResetAllInputsFieldResena()
    {
        InputPuntoInicial.text = "";
        InputPuntoFinal.text = "";
        InputPuntoFallo.text = "";
        InputMotivo.text = "";
    }
    IEnumerator SendResena(string inicio, string final, string puntoFallo, string motivo)
    {
        Debug.Log("ENTRAMOS A MANDAR A BD");

        const string IP = "144.22.42.236";
        const string port = "3000";
        const string baseURI = "http://"+IP+":"+port+"/api/";
        // Crear formulario
        WWWForm form = new WWWForm();
        form.AddField("inicio", inicio);
        form.AddField("final", final);
        form.AddField("puntofallo", puntoFallo);
        form.AddField("motivo", motivo);
        //Realizar request
        UnityWebRequest www = UnityWebRequest.Post(baseURI+"resena/add", form);
        yield return www.SendWebRequest();
        // Resolucion de la request
        if (www.result != UnityWebRequest.Result.Success)
        {
            
            Debug.Log("Error post: "+ www.error);
            //_ShowAndroidToastMessage("Error al enviar los datos");

        }
        else
        {
            MensajeResena.text = "El reporte ha sido realizado correctamente.";
            Debug.Log("Form upload complete!");
            //Mensaje.text = "Su Sugerencia ha sido enviada correctamente.";
            //_ShowAndroidToastMessage("Datos guardados");
        }
        yield break;
    }
}
