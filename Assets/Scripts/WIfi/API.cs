using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class API : MonoBehaviour
{
    [SerializeField]
    List<WifiData> wifis;
    
    public GameObject Text;
    [SerializeField]
    GameObject contenido;

    static int SortByMac(WifiData w1, WifiData w2){
        return w1.mac.CompareTo(w2.mac);
    }

    static int SortByFloor(WifiData w1, WifiData w2){
        return w1.floor.CompareTo(w2.floor);
    }

    IEnumerator SendPoints(int floor, string macs, string intensities)
    {
        // const string IP = "144.22.42.236";
        const string IP = "localhost";
        const string port = "3000";
        const string baseURI = "http://"+IP+":"+port+"/api/";
        // Crear formulario
        WWWForm form = new WWWForm();
        form.AddField("macs", macs);
        form.AddField("intensities", intensities);
        form.AddField("floor", floor);
        //Realizar request
        UnityWebRequest www = UnityWebRequest.Post(baseURI+"wifi/add", form);
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
    }

    IEnumerator FindPointData(string name){
        // const string IP = "144.22.42.236";
        const string IP = "localhost";
        const string port = "3000";
        const string baseURI = "http://"+IP+":"+port+"/api/";
        WWWForm form = new WWWForm();
        form.AddField("name", name);
        UnityWebRequest www =  UnityWebRequest.Post(baseURI+"points/find", form);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error post: "+ www.error);
        }
        else
        {
            // Recuperar JSON
            string response = www.downloadHandler.text;
            // Transformar JSON a Point
            Point point = JsonUtility.FromJson<Point>(response);
            Debug.Log(point.x);
            Debug.Log(point.y);
        }
    }

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

    void crearDatos(){
        wifis = new List<WifiData>();
        // Ejemplo wifi piso 1
        wifis.Add(new WifiData("2A:3C",2,1));
        wifis.Add(new WifiData("1C:3C",2,1));
        wifis.Add(new WifiData("1A:3C",2,1));
        // Ejemplo wifi piso 2
        wifis.Add(new WifiData("2A:3C",1,2));
        wifis.Add(new WifiData("1C:3C",1,2));
        wifis.Add(new WifiData("1A:3C",1,2));

        wifis.Sort(SortByFloor);
        // Crear dataset
        Dictionary<int, List<WifiData>> dataset = new Dictionary<int, List<WifiData>>();
        foreach (WifiData wifi in wifis)
        {
            if(!dataset.ContainsKey(wifi.floor)){
                dataset[wifi.floor] = new List<WifiData>();
            }
            dataset[wifi.floor].Add(wifi);
        }
        // Revisar dataset
        foreach(var (floor, data) in dataset){
            Debug.Log(floor);
            List<string> macs = new List<string>();
            List<string> intensities = new List<string>();
            data.Sort(SortByMac);
            foreach(WifiData wifi in data){
                macs.Add(wifi.mac);
                intensities.Add(wifi.intensity.ToString());
            }
            string _macs = string.Join(",",macs);
            string _intensities = string.Join(",",intensities);
            // Llamar API con los datos.
            StartCoroutine(SendPoints(floor,_macs,_intensities));
        }
    }

    public void Start(){
        StartCoroutine(DestinosDisponibles());
    }
}
