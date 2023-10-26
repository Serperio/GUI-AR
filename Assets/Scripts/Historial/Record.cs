using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Record : MonoBehaviour
{
    List<string> userFrequentRequests;
    [SerializeField]
    HistorialBehaviour hist;
    string userMail;
    [SerializeField]
    TMP_InputField inputField;
    [SerializeField]
    TMP_Dropdown tmpDrop;
    string request;

    List<string> masBuscados = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        userMail = GameObject.Find("Correo").GetComponent<nValue>().correoUsuario;
        userFrequentRequests = new List<string>();
        listShow();
    }

    public void listShow()
    {
        GetFrequentUserRequests();
    }

    public void SendUserRequests()
    {
        request = inputField.text;
        if (!string.IsNullOrEmpty(request))
        {
        print("Sending Dataaa: Correo"+ userMail+ ", Request"+ request);
        WWWForm www = new WWWForm();
        www.AddField("mail", userMail);
        www.AddField("request", request);
        StartCoroutine(APIHelper.POST("frecuencia/add", www, response => { Debug.Log("Enviado con exito"); }));
        }
    }


    public void GetFrequentUserRequests()
    {
        StartCoroutine(APIHelper.GET("frecuencia/"+userMail, response => {
            List<string> requests = API.listJson(response);
            print(response);
            List<Frecuencia> frecuencias = new List<Frecuencia>();
            foreach(string json in requests)
            {
                frecuencias.Add(JsonUtility.FromJson<Frecuencia>(json));
            }
            masBuscados = new List<string>();
            foreach (Frecuencia frecuencia in frecuencias)
            {
                print("Las frecuencias son:" + frecuencia._id);
                masBuscados.Add(frecuencia._id);
            }
            tmpDrop.ClearOptions();
            userFrequentRequests = masBuscados;
            tmpDrop.AddOptions(userFrequentRequests);
        }));
    }

    public void DeleteAllUserRequests()
    {
        WWWForm www = new WWWForm();
        StartCoroutine(APIHelper.POST("frecuencia/" + userMail+"/delete", www, response => {
            Debug.Log("Borrado exitoso");
        }));
    }

    public class Frecuencia
    {
        public string _id;
        public string count;
    }
}
