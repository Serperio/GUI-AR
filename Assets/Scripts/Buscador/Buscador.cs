using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

public class Buscador : MonoBehaviour
{
    /*
     Buscador para encontrar algun destino con nombre
    */

    [SerializeField]
    GameObject contenido; //Donde se mostraran los datos
    [SerializeField] 
    GameObject Text; //Campo del Input
    [SerializeField]
    GameObject textoPunto; //Texto donde se escribe la informacion
    [SerializeField]
    GameObject listado; // Scroll view para seleccionar los vecinos
    [SerializeField]
    GameObject prefabToggle; // Objeto que permite seleccionar vecinos
    [SerializeField]
    TMP_InputField inputNombre;
    [SerializeField]
    TMP_InputField inputDescripcion;
    [SerializeField]
    TextMeshProUGUI inputTipo;
    private float lastX;
    private float lastY;
    private int lastFloor;
    private string nombreAntiguo;

    [SerializeField]
    GameObject listaDeContenido;
    [SerializeField]
    GameObject botonDeLaListita;

    string inputText; //texto que hay en el Gameobject Text

    static List<string> JsonToList(string jsonData){ //Convierte Json a Lista
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

    IEnumerator DestinosDisponibles(){ //Obtiene todos los destinos desde la base de datos
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
            List<string> data = JsonToList(response);
            // Transformar JSON a Point
            List<Point> points = new List<Point>();
            foreach(string dato in data){
                points.Add(JsonUtility.FromJson<Point>(dato));
            }
            // Entregar resultados
            foreach(Point point in points)
            {
                if(point.tipo != "especial"){
                    GameObject texto = Instantiate(Text, Vector3.zero, Quaternion.identity);
                    texto.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = point.name;
                    texto.transform.parent = contenido.transform;
                    texto.transform.localPosition = Vector3.zero;
                    texto.transform.localScale = Vector3.one;
                    Debug.Log(point.tipo);
                }
            }
        }
    }

    public void GetInput(TMP_InputField s){ //Obtener valor input
        inputText = s.text;
        StartCoroutine(FindPointData(inputText));
    }

    IEnumerator FindPointData(string name) //Buscar los datos de un punto por nombre
    {
        Debug.Log("Nombre punto: "+name);
        //const string IP = "144.22.42.236";
        const string IP = "localhost";
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
            string output = ""; // texto a mostrar en el "textoPunto"
            Point point = null;
            try
            {
                // Recuperar JSON
                string response = www.downloadHandler.text;
                Debug.Log("Log response:"+www.downloadHandler.text);
                // Transformar JSON a Point
                point = JsonUtility.FromJson<Point>(response);
                Debug.Log("Log point");
                inputNombre.text = point.name;
                inputDescripcion.text = point.description;
                inputTipo.text = point.tipo;
                lastX = point.x;
                lastY = point.y;
                lastFloor = point.floor;
                nombreAntiguo = point.name;
                output = "Latitud: " + point.x + "\nLongitud: " + point.y + "\nPiso: " + point.floor;
                Debug.Log("info");
            } catch
            {
                Debug.Log("error try");
                output = "Destino no valido, por favor indicar otro destino";
            }
            // Obtener nombres de todos los vecinos
            if(point != null)
            {
                Debug.Log("vecinos punto:" + point.vecinos.Length);
                foreach(string id in point.vecinos)
                {
                    UnityWebRequest info = UnityWebRequest.Get(baseURI + "points/" + id);
                    yield return info.SendWebRequest();
                    if(info.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogError("Error Get: " + info.error);
                    }
                    else
                    {
                        Point p = JsonUtility.FromJson<Point>(info.downloadHandler.text);
                        GameObject instancia = Instantiate(prefabToggle, Vector3.zero, Quaternion.identity);
                        instancia.GetComponent<RectTransform>().transform.SetParent(listado.transform);
                        instancia.transform.localScale = Vector3.one;
                        instancia.GetComponentInChildren<Text>().text = p.name;
                        Debug.Log("AQUI ERA LA WEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
                        Debug.Log("WENAAAAAAAAAA PO VIEJA QLA MARACAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA"+p.tipo);
                    }
                }
            }
            // Mostrar informacion
            OpenDetalle();
            //textoPunto.GetComponentInChildren<TextMeshProUGUI>().text = output;

        }
    }

    //Dsps Cambiar a UIbehaviour, (comportamiento de botones)
    public void BuscarSitiosDisponibles(){
        StartCoroutine(DestinosDisponibles());
    }

    public void FiltrarPorNombre(TMP_InputField texto)
    {
        StartCoroutine(APIHelper.GET("points", response =>
        {
            List<string> jsonPoints = API.listJson(response);
            List<Point> puntos = new List<Point>(); 
            foreach(string json in jsonPoints)
            {
                puntos.Add(JsonUtility.FromJson<Point>(json));
            }

            string nombre = texto.text;
            // Limpiar la listita de contenido
            foreach (Transform child in listaDeContenido.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Point p in puntos)
            {
                if (p.name.ToLower() == nombre.ToLower())
                {
                    foreach (Transform child in listaDeContenido.transform)
                    {
                        Destroy(child.gameObject);
                    }
                    // Agregarlo a la listita de contenido
                    GameObject texto2 = Instantiate(botonDeLaListita, Vector3.zero, Quaternion.identity);
                    texto2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = p.name;
                    texto2.transform.parent = listaDeContenido.transform;
                    texto2.transform.localPosition = Vector3.zero;
                    texto2.transform.localScale = Vector3.one;
                }
                else if (string.IsNullOrEmpty(nombre))
                {
                    GameObject texto2 = Instantiate(botonDeLaListita, Vector3.zero, Quaternion.identity);
                    texto2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = p.name;
                    texto2.transform.parent = listaDeContenido.transform;
                    texto2.transform.localPosition = Vector3.zero;
                    texto2.transform.localScale = Vector3.one;
                }
            }
        }));
    }

    public void ActualizarPunto()
    {
        Debug.Log("Actualizando punto...");
        API api = GameObject.Find("AppManager").GetComponent<API>(); // Recuperar API
        GameObject[] toggles = GameObject.FindGameObjectsWithTag("Vecino");
        List<string> vecinos = new List<string>();
        Debug.Log("Toggles: "+toggles.Length);
        foreach (GameObject toggle in toggles)
        {
            if (toggle.GetComponent<Toggle>().isOn)
            {
                string vecino = toggle.GetComponentInChildren<Text>().text;
                vecinos.Add(vecino);
            }
        }
        string _vecinos = string.Join(",", vecinos);
        Debug.Log("Llamando a actualizar");
        api.ActualizarPuntoDB(inputNombre.text, nombreAntiguo ,inputDescripcion.text, inputTipo.text, _vecinos, lastX, lastY, lastFloor);
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
}
