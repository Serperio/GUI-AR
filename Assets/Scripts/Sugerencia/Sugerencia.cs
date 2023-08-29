using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

public class Sugerencia : MonoBehaviour
{
    [SerializeField] 
    public TMP_InputField Piso;
    [SerializeField] 
    public TMP_InputField Edificio;
    [SerializeField] 
    public TMP_InputField NombreTentativo;
    [SerializeField] 
    public TMP_InputField Text;
    [SerializeField] 
    public TextMeshProUGUI MensajeSugerencia;
    // Start is called before the first frame update
    IEnumerator SendSugerencia(int piso, string edificio, string texto, string nombretentativo)
    {
        Debug.Log("ENTRAMOS A MANDAR A BD");

        const string IP = "144.22.42.236";
        const string port = "3000";
        const string baseURI = "http://"+IP+":"+port+"/api/";
        // Crear formulario
        WWWForm form = new WWWForm();
        form.AddField("piso", piso);
        form.AddField("edificio", edificio);
        form.AddField("nombretentativo", nombretentativo);
        form.AddField("motivo", texto);
        //Realizar request
        UnityWebRequest www = UnityWebRequest.Post(baseURI+"sugerencia/add", form);
        yield return www.SendWebRequest();
        // Resolucion de la request
        if (www.result != UnityWebRequest.Result.Success)
        {
            
            Debug.Log("Error post: "+ www.error);
            //_ShowAndroidToastMessage("Error al enviar los datos");
        }
        else
        {
            Debug.Log("Form upload complete!");
            MensajeSugerencia.text = "Su Sugerencia ha sido enviada correctamente.";
            //_ShowAndroidToastMessage("Datos guardados");
        }
        yield break;
    }
    public void GeneraSugerencia()
    {
        MensajeSugerencia.text = "Su sugerencia ha fallado, intente nuevamente.";
        int piso = int.Parse(Piso.text);
        string edificio = Edificio.text;
        string texto = Text.text;
        string nombretentativo = NombreTentativo.text;
        SendSugerencia(piso, edificio, texto, nombretentativo);
    }

    public void ResetAllInputsFieldSugerencia()
    {
        Piso.text = "";
        Edificio.text = "";
        NombreTentativo.text = "";
        Text.text = "";
    }
}
