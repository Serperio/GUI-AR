using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class Buscador : MonoBehaviour
{

    [SerializeField]
    GameObject contenido;
    [SerializeField] 
    GameObject Text;
    [SerializeField]
    GameObject textoPunto;

    string inputText;

    static List<string> listJson(string jsonData){
        string json = jsonData.Substring(1,jsonData.Length-2);            
        bool startParentesis = false;
        bool endParentesis = false;
        string dato = "";
        List<string> strings = new List<string>();
        foreach(char caracter in json){
            if(caracter == '{'){
                dato = "";
                startParentesis = true;
            } else if(caracter == '}'){
                endParentesis = true;
            }
            if(startParentesis){
                dato += caracter;
            }
            if(endParentesis){
                startParentesis = false;
                endParentesis = false;
                strings.Add(dato);
            }
        }
        return strings;
    }

    IEnumerator DestinosDisponibles(){
        const string IP = "144.22.42.236";
        //const string IP = "localhost";
        const string port = "3000";
        const string baseURI = "http://"+IP+":"+port+"/api/";

        UnityWebRequest www =  UnityWebRequest.Get(baseURI+"points");
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error post: "+ www.error);
        }
        else
        {
            // Recuperar JSON
            string response = www.downloadHandler.text;
            // Obtener listado de puntos
            List<string> data = listJson(response);
            // Transformar JSON a Point
            List<Point> points = new List<Point>();
            foreach(string dato in data){
                points.Add(JsonUtility.FromJson<Point>(dato));
            }
            // Entregar resultados
            foreach(Point point in points)
            {
                GameObject texto = Instantiate(Text, Vector3.zero, Quaternion.identity);
                texto.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = point.name;
                texto.transform.parent = contenido.transform;
                texto.transform.localPosition = Vector3.zero;
                texto.transform.localScale = Vector3.one;
                Debug.Log(point.name);
            }
        }
    }

    public void GetInput(TMP_InputField s){
        inputText = s.text;
        StartCoroutine(FindPointData(inputText));
    }

    IEnumerator FindPointData(string name)
    {
        Debug.Log("Nombre punto: "+name);
        const string IP = "144.22.42.236";
        //const string IP = "localhost";
        const string port = "3000";
        const string baseURI = "http://" + IP + ":" + port + "/api/";
        WWWForm form = new WWWForm();
        form.AddField("name", name);
        UnityWebRequest www = UnityWebRequest.Post(baseURI + "points/find", form);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error post: " + www.error);
        }
        else
        {
            try
            {
                // Recuperar JSON
                string response = www.downloadHandler.text;
                // Transformar JSON a Point
                Point point = JsonUtility.FromJson<Point>(response);
                OpenDetalle();
                textoPunto.GetComponentInChildren<TextMeshProUGUI>().text = "Latitud: " + point.x + "\nLongitud: " + point.y;
            } catch
            {
                OpenDetalle();
                textoPunto.GetComponentInChildren<TextMeshProUGUI>().text = "Destino no valido, por favor indicar otro destino";
            }
            
        }
    }

    public void BuscarSitiosDisponibles(){
        StartCoroutine(DestinosDisponibles());
    }

    public void DetalleSitio(TextMeshProUGUI texto)
    {
        StartCoroutine(FindPointData(texto.text));
    }

    public void OpenDetalle()
    {
        textoPunto.SetActive(true);
    }

    public void CloseDetalle()
    {
        textoPunto.SetActive(false);
    }
    public void Sitio()
    {
        StartCoroutine(FindPointData(gameObject.GetComponentInChildren<TextMeshProUGUI>().text));
    }
}
