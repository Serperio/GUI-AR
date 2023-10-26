using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Record : MonoBehaviour
{
    List<string> userFrequentRequests;
    [SerializeField]
    HistorialBehaviour hist;
    string userMail;

    List<string> masBuscados = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        userFrequentRequests = new List<string>();
        userMail = hist.correoVal;
       listShow();
    }

    public void listShow()
    {
        GetFrequentUserRequests();
    }

    public void SendUserRequests(string request)
    {
        WWWForm www = new WWWForm();
        www.AddField("mail", userMail);
        www.AddField("request", request);
        StartCoroutine(APIHelper.POST("frecuencia/add", www, response => { Debug.Log("Enviado con exito"); }));
    }


    public void GetFrequentUserRequests()
    {
        StartCoroutine(APIHelper.GET("frecuencia/"+userMail, response => {
            List<string> requests = API.listJson(response);
            List<Frecuencia> frecuencias = new List<Frecuencia>();
            foreach(string json in requests)
            {
                frecuencias.Add(JsonUtility.FromJson<Frecuencia>(json));
            }
            masBuscados = new List<string>();
            foreach (Frecuencia frecuencia in frecuencias)
            {
                masBuscados.Add(frecuencia.request);
            }
            hist.SugerenciasAPI = masBuscados;
        }));
    }

    public void DeleteAllUserRequests()
    {
        WWWForm www = new WWWForm();
        StartCoroutine(APIHelper.POST("frecuencia/" + userMail, www, response => {
            Debug.Log("Borrado exitoso");
        }));
    }

    public class Frecuencia
    {
        public string mail;
        public string request;
    }
}
