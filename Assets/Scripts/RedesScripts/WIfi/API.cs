using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

public class API : MonoBehaviour
{
    [SerializeField]
    EditDropdown editDropdown;
    // Variables
    [SerializeField]
    TextMeshProUGUI npiso;
    [SerializeField]
    TextMeshProUGUI npiso2;
    [SerializeField]
    TMP_InputField inputCambiarPiso;

    [SerializeField]
    List<WifiData> wifis;

    [SerializeField]
    FixedFormatoWifi fixedWifi;

    List<string> wifisMAC= new List<string>();
    List<string> wifisMACRef = new List<string>();
    List<int> wifisIntensidades = new List<int>();

    List<int> wifisIntensidadesfixed;

    public GameObject Text;
    [SerializeField]
    GameObject contenido;

    [SerializeField]
    Button myButton;
    [SerializeField]
    public TMP_InputField myInputField;
    private int inputValue;

    [SerializeField]
    public TextMeshProUGUI tipoInput;
    [SerializeField]
    public TMP_InputField pisoInput;
    [SerializeField]
    public TMP_InputField nameInput;
    [SerializeField]
    TMP_InputField borrarInput;
    [SerializeField]
    public TMP_InputField pisoDatasetInput; //Input de piso para guardar las redes wifi en la DB
    [SerializeField]
    public TMP_InputField edificioDatasetInput; //Input de eficio para guardar las redes wifi en la DB
    [SerializeField]
    public TextMeshProUGUI estadoWifiDB; //Texto para mostrar el estado del guardado de datos de wifi en la DB
    [SerializeField]
    TMP_InputField descripcionInput;

    [SerializeField]
    public MyPositionGPS GPS_handler; // Deberia ser singleton
    [SerializeField]
    GameObject prefabNearby;
    [SerializeField]
    GameObject NearbyScroll;

    string pisoManual="-10";

    public List<Point> _pointlist = new List<Point>();
    public Prediccion pred;

    // Guardar datos para generar dataset de puntos wifi
    //  asociados a un piso en la coleccion beta
    public void GenerarDataset()
    {
        int piso = int.Parse(pisoDatasetInput.text);
        string macs = string.Join(",", wifisMAC);
        string intensidades = string.Join(",", wifisIntensidades);
        string edificio = edificioDatasetInput.text;

        WWWForm form = new WWWForm();
        form.AddField("macs", macs);
        form.AddField("intensities", intensidades);
        form.AddField("floor", piso);
        form.AddField("edificio", edificio);
        //Realizar request
        StartCoroutine(APIHelper.POST("beta/add", form, (string response)=> {
            //Mostrar pop up de estado
            estadoWifiDB.text = "Enviado con exito";
            Debug.Log("Enviado con exito");
        }, (string error)=> {
            // Mostrar pop up de error
            estadoWifiDB.text = "Error en el envio:\n"+error;
            Debug.Log("Error: " + error);
        }));
    }

    IEnumerator guardarPunto()
    {
        Debug.Log("Guardando punto...");
        // Crear formulario
        WWWForm form = new WWWForm();
        Utilities._ShowAndroidToastMessage("Guardando punto...");
        // Obtener posicion espacial
        string xPos = GPS_handler.GetLastPosition()[0].ToString();
        string yPos = GPS_handler.GetLastPosition()[1].ToString();
        // Request para guardar el punto
        form.AddField("x", xPos);
        form.AddField("y", yPos);
        form.AddField("floor", pisoInput.text);
        form.AddField("tipo", tipoInput.text);
        form.AddField("name", nameInput.text);
        form.AddField("description", descripcionInput.text);
        yield return StartCoroutine(APIHelper.POST("points/add", form));

        GuardarVecinos(nameInput.text);
    }

    void GuardarVecinos(string origen)
    {
        // Obtener puntos cercanos
        GameObject[] toggles = GameObject.FindGameObjectsWithTag("NearbyPoint");
        List<string> vecinos = new List<string>();
        // Guardar listado de puntos seleccionados
        foreach (GameObject toggle in toggles)
        {
            if (toggle.GetComponent<Toggle>().isOn)
            {
                string vecino = toggle.GetComponentInChildren<Text>().text;
                vecinos.Add(vecino);
            }
        }

        //TODO: quitar canvas de agregar punto luego de tener los nombres de los puntos.

        // Solo guardar vecinos si realmente hay vecinos
        if (vecinos.Count > 0)
        {
            string _vecinos = string.Join(",", vecinos);
            WWWForm form = new WWWForm();
            form.AddField("origen", origen);
            form.AddField("vecinos", _vecinos);
            Utilities._ShowAndroidToastMessage("Guardando vecinos");
            StartCoroutine(APIHelper.POST("points/addArc", form));
        }

        
    }

    IEnumerator actualizarPunto(string name, string nombreAntiguo, string description, string tipo, string vecinos, float x, float y, int piso)
    {
        // Request para borrar punto
        WWWForm form = new WWWForm();
        yield return StartCoroutine(APIHelper.POST("points/" + nombreAntiguo + "/delete", form));
        // Request para guardar el punto
        WWWForm form2 = new WWWForm();
        form2.AddField("x", x.ToString().Replace(",", "."));
        form2.AddField("y", y.ToString().Replace(",", "."));
        form2.AddField("floor", piso.ToString());
        form2.AddField("tipo", tipo);
        form2.AddField("name", name);
        form2.AddField("description", description);
        yield return StartCoroutine(APIHelper.POST("points/add", form2, response => {
            editDropdown.CargarMenuEditar();
        }));
        //Request para guardar vecinos
        WWWForm form3 = new WWWForm();
        form3.AddField("origen", name);
        form3.AddField("vecinos", vecinos);
        Utilities._ShowAndroidToastMessage("Actualizando vecinos...");
        StartCoroutine(APIHelper.POST("points/addArc", form3));
        
    }

    public void ActualizarPuntoDB(string name, string nombreAntiguo, string description, string tipo, string vecinos, float x, float y, int piso)
    {
        StartCoroutine(actualizarPunto(name, nombreAntiguo, description, tipo, vecinos, x, y, piso));
    }

    public void NearbyPointsListAPI()
    {
        // Obtener ubicacion actual
        string xPos = GPS_handler.GetLastPosition()[0].ToString();
        string yPos = GPS_handler.GetLastPosition()[1].ToString();
        // Crear formulario
        WWWForm form = new WWWForm();
        // TODO: Reemplazar por xPos e yPos cuando sea un entorno real
        //form.AddField("x", "-33.0348");
        //form.AddField("y", "-71.59656");
        form.AddField("x", xPos);
        form.AddField("y", yPos);
        // Pedir listado de puntos cercanos
        StartCoroutine(APIHelper.POST("points/nearby", form, response => {
            List<string> points = listJson(response);
            foreach (Transform child in NearbyScroll.transform)
            {
                Destroy(child.gameObject);
            }
            for (int i = 0; i < points.Count; i++)
            {
                Point point = JsonUtility.FromJson<Point>(points[i]);
                GameObject instancia = Instantiate(prefabNearby, Vector3.zero, Quaternion.identity);
                instancia.transform.SetParent(NearbyScroll.transform);
                instancia.transform.localScale = Vector3.one;
                instancia.transform.position = Vector3.zero;
                instancia.GetComponentInChildren<Text>().text = point.name;
            }
        }));
    }
    public void NearbyPointsAPI()
    {
        List<Point> auxList = new List<Point>();
        // Obtner ubicacion actual
        string xPos = GPS_handler.GetLastPosition()[0].ToString();
        string yPos = GPS_handler.GetLastPosition()[1].ToString();
        // Crear formulario
        WWWForm form = new WWWForm();
        form.AddField("x", xPos);
        form.AddField("y", yPos);
        // Pedir listado de puntos cercanos
        StartCoroutine(APIHelper.POST("points/nearby", form, response =>
        {
            List<string> points = listJson(response);
            for (int i = 0; i < points.Count; i++)
            {
                Point point = JsonUtility.FromJson<Point>(points[i]);
                auxList.Add(point);
            }
            _pointlist = auxList;
        }));
    }

    public void GuardarPuntoDB(){
        StartCoroutine(guardarPunto());
    }

    public void BorrarPuntoDB(){
        WWWForm form = new WWWForm();
        StartCoroutine(APIHelper.POST("points/" + borrarInput.text + "/delete", form, (response)=> {
            editDropdown.CargarMenuEditar();
        }));
    }

    public void getWifis(List<Network> wifis){
        List<string> wifiMacAux = new List<string>();
        List<int> wifisIntensidadesAux = new List<int>();
        foreach (Network wifi in wifis){
            wifiMacAux.Add(wifi.SSID);
            wifisIntensidadesAux.Add(wifi.signalLevel);
        }
        wifisMAC = wifiMacAux;
        wifisIntensidades = wifisIntensidadesAux;
        StartCoroutine(WifiDisponibles());
    }
    // Agregar a Utilities
    static public List<string> listJson(string jsonData){
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

    public void DestinosDisponibles(){
        // Limpiar lista
        foreach(Transform child in contenido.transform)
        {
            Destroy(child.gameObject);
        }
        // Actualizar lista
        StartCoroutine(APIHelper.GET("points", response => {
            // Obtener listado de puntos
            List<string> data = listJson(response);
            // Transformar JSON a Point
            List<Point> points = new List<Point>();
            foreach (string dato in data)
            {
                points.Add(JsonUtility.FromJson<Point>(dato));
            }
            // Entregar resultados
            foreach (Point point in points)
            {
                if (point.tipo != "especial")
                {
                    GameObject texto = Instantiate(Text, Vector3.zero, Quaternion.identity);
                    texto.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = point.name;
                    texto.transform.parent = contenido.transform;
                    texto.transform.localPosition = Vector3.zero;
                    texto.transform.localScale = Vector3.one;
                }
            }
        }));
    }
    public void cambiarPisoManual(TextMeshProUGUI a)
    {
        pisoManual = a.text;
        print("pisoManual: " + pisoManual);
        npiso.text = "Numero de piso: " + pisoManual;
        npiso2.text = "Numero de piso: " + pisoManual;
    }

    IEnumerator WifiDisponibles(){
        if (pisoManual != "-10")
        {
            npiso.text = "Numero de piso: " +pisoManual;
            npiso2.text = "Numero de piso: " + pisoManual;
            yield return new WaitForSeconds(10);
            pisoManual = "-10";
            StartCoroutine(WifiDisponibles());

        }
        else
        {
        // Obtener redes wifi conocidas
        yield return StartCoroutine(APIHelper.GET("wifi", response => {
            // Borrar lista cuando se llama
            wifisMACRef = new List<string>();
            // Obtener listado de puntos
            List<string> data = listJson(response);
            // Transformar JSON a Point
            List<WifiJson> points = new List<WifiJson>();
            foreach (string dato in data)
            {
                points.Add(JsonUtility.FromJson<WifiJson>(dato));
            }
            // Entregar resultados
            foreach (string mac in points[0].macs)
            {
                wifisMACRef.Add(mac);
            }
        }));
        // Corregir listado de redes wifi
        //List<string> testRef = new List<string> { "0c:b6:d2:0b:18:b0", "18:35:d1:90:6e:0f", "18:35:d1:b0:54:67", "18:64:72:77:29:13", "18:64:72:77:29:15", "2a:62:f8:2e:71:d4", "48:d3:43:13:6f:f1", "48:d3:43:32:c9:59", "50:c7:bf:e0:ad:22", "5c:df:89:3a:bd:f8", "5c:df:89:7a:bd:f8", "5c:df:89:ba:bd:f8", "5e:05:c0:b1:0d:56", "68:3a:48:1d:80:c9", "68:3a:48:1d:9e:2a", "8a:29:9c:f8:51:4d", "b8:c3:85:0e:89:d1", "c8:67:5e:43:6f:64", "c8:67:5e:43:6f:65", "c8:67:5e:43:6f:66", "c8:67:5e:43:6f:67", "c8:67:5e:43:6f:68", "c8:67:5e:44:73:94", "c8:67:5e:44:73:95", "c8:67:5e:44:73:96", "c8:67:5e:44:73:97", "c8:67:5e:44:73:98", "c8:67:5e:44:73:a4", "c8:67:5e:44:73:a5", "c8:67:5e:44:73:a6", "c8:67:5e:44:73:a7", "c8:67:5e:44:73:a8", "c8:67:5e:e4:7d:e5", "c8:67:5e:e4:7d:e6", "c8:67:5e:e4:7d:e7", "c8:67:5e:e4:7d:e8", "c8:67:5e:e4:8c:24", "c8:67:5e:e4:8c:25", "c8:67:5e:e4:8c:26", "c8:67:5e:e4:8c:27", "c8:67:5e:e4:8c:28", "c8:67:5e:e4:8f:e4", "c8:67:5e:e4:8f:e5", "c8:67:5e:e4:8f:e6", "c8:67:5e:e4:8f:e7", "c8:67:5e:e4:8f:e8", "c8:67:5e:e4:94:64", "c8:67:5e:e4:94:65", "c8:67:5e:e4:94:66", "c8:67:5e:e4:94:67", "c8:67:5e:e4:94:68", "c8:67:5e:e6:da:64", "c8:67:5e:e6:da:65", "c8:67:5e:e6:da:66", "c8:67:5e:e6:da:67", "c8:67:5e:e6:da:68", "c8:67:5e:e6:e3:e4", "c8:67:5e:e6:e3:e5", "c8:67:5e:e6:e3:e6", "c8:67:5e:e6:e3:e7", "c8:67:5e:e6:e3:e8", "c8:67:5e:e7:3a:24", "c8:67:5e:e7:3a:25", "c8:67:5e:e7:3a:26", "c8:67:5e:e7:3a:27", "c8:67:5e:e7:3a:28", "f4:ea:b5:3f:48:64", "f4:ea:b5:3f:48:65", "f4:ea:b5:3f:48:66", "f4:ea:b5:3f:48:67", "f4:ea:b5:3f:48:68", "f4:ea:b5:3f:50:65", "f4:ea:b5:3f:50:66", "f4:ea:b5:3f:50:67", "f4:ea:b5:3f:50:68", "f4:ea:b5:3f:56:64", "f4:ea:b5:3f:56:65", "f4:ea:b5:3f:56:66", "f4:ea:b5:3f:56:67", "f4:ea:b5:3f:56:68", "f4:ea:b5:3f:5a:64", "f4:ea:b5:3f:5a:65", "f4:ea:b5:3f:5a:66", "f4:ea:b5:3f:5a:67", "f4:ea:b5:3f:5a:68", "f4:ea:b5:3f:83:a4", "f4:ea:b5:3f:83:a5", "f4:ea:b5:3f:83:a6", "f4:ea:b5:3f:83:a7", "f4:ea:b5:3f:83:a8", "f4:ea:b5:3f:86:25", "f4:ea:b5:3f:86:26", "f4:ea:b5:3f:86:27", "f4:ea:b5:3f:86:28", "f4:ea:b5:3f:89:24", "f4:ea:b5:3f:89:25", "f4:ea:b5:3f:89:26", "f4:ea:b5:3f:89:27", "f4:ea:b5:3f:89:28", "f4:ea:b5:3f:a5:17", "f4:ea:b5:3f:a5:24", "f4:ea:b5:3f:a5:25", "f4:ea:b5:3f:a5:26", "f4:ea:b5:3f:a5:27", "f4:ea:b5:3f:a5:28", "f4:ea:b5:3f:aa:64", "f4:ea:b5:3f:aa:65", "f4:ea:b5:3f:aa:66", "f4:ea:b5:3f:aa:67", "f4:ea:b5:3f:aa:68", "f4:ea:b5:a7:35:64", "f4:ea:b5:a7:35:65", "f4:ea:b5:a7:35:66", "f4:ea:b5:a7:35:67", "f4:ea:b5:a7:35:68", "f4:ea:b5:a7:37:14", "f4:ea:b5:a7:37:15", "f4:ea:b5:a7:37:16", "f4:ea:b5:a7:37:17", "f4:ea:b5:a7:37:18", "f4:ea:b5:a7:37:24", "f4:ea:b5:a7:37:25", "f4:ea:b5:a7:37:26", "f4:ea:b5:a7:37:27", "f4:ea:b5:a7:37:28", "f4:ea:b5:a7:4a:24", "f4:ea:b5:a7:4a:25", "f4:ea:b5:a7:4a:26", "f4:ea:b5:a7:4a:27", "f4:ea:b5:a7:4a:28", "f4:ea:b5:a7:55:a5", "f4:ea:b5:a7:55:a6", "f4:ea:b5:a7:55:a7", "f4:ea:b5:a7:55:a8", "f4:ea:b5:a7:5a:24", "f4:ea:b5:a7:5a:25", "f4:ea:b5:a7:5a:26", "f4:ea:b5:a7:5a:27", "f4:ea:b5:a7:5a:28", "f4:ea:b5:a7:5e:15", "f4:ea:b5:a7:5e:16", "f4:ea:b5:a7:5e:17", "f4:ea:b5:a7:5e:18", "f4:ea:b5:a7:62:64", "f4:ea:b5:a7:62:65", "f4:ea:b5:a7:62:66", "f4:ea:b5:a7:62:67", "f4:ea:b5:a7:62:68", "f4:ea:b5:a7:64:a4", "f4:ea:b5:a7:64:a5", "f4:ea:b5:a7:64:a6", "f4:ea:b5:a7:64:a7", "f4:ea:b5:a7:64:a8", "f4:ea:b5:c8:00:a4", "f4:ea:b5:c8:00:a5", "f4:ea:b5:c8:00:a6", "f4:ea:b5:c8:00:a7", "f4:ea:b5:c8:00:a8", "f4:ea:b5:f8:47:18", "f8:d2:ac:45:0c:e0", "fa:d0:27:ac:42:e9" };
        //List<string> testMAC = new List<string> { "c8:67:5e:e6:de:e5", "c8:67:5e:e6:de:e6", "c8:67:5e:e6:de:e7", "c8:67:5e:e6:de:e8", "c8:67:5e:e6:de:e4", "c8:67:5e:44:73:95", "c8:67:5e:44:73:98", "c8:67:5e:44:73:96", "c8:67:5e:44:73:97", "c8:67:5e:44:73:94", "5c:df:89:7a:bd:f8", "5c:df:89:ba:bd:f8", "5c:df:89:3a:bd:f8", "f4:ea:b5:a7:3e:d7", "c8:67:5e:e4:8c:25", "c8:67:5e:e4:8c:26", "f4:ea:b5:a7:3e:d5", "f4:ea:b5:a7:3e:d6", "c8:67:5e:e4:8c:28", "c8:67:5e:e4:8c:27", "68:3a:48:1d:9e:2a", "8a:44:03:d9:32:00", "c8:67:5e:e6:da:67", "c8:67:5e:e6:da:65", "f4:ea:b5:3f:7f:67", "f4:ea:b5:3f:7f:65", "f4:ea:b5:3f:7f:66", "f4:ea:b5:3f:7f:68", "c8:67:5e:e7:3a:25", "f4:ea:b5:3f:7f:64", "68:3a:48:1d:81:5e", "ba:bb:7c:07:a3:27", "68:3a:48:1d:80:c9" };
        //List<int> testIntensidades = new List<int> { 18, 18, 18, 18, 18, 15, 15, 15, 15, 15, 14, 14, 14, 12, 11, 11, 11, 11, 11, 11, 9, 8, 7, 7, 7, 7, 7, 7, 6, 6, 5, 5, 4 };

        wifisIntensidadesfixed = fixedWifi.generarVector(wifisMACRef, wifisMAC,wifisIntensidades);
        //wifisIntensidadesfixed = fixedWifi.generarVector(testRef, testMAC, testIntensidades);
        Debug.Log("DEV intensidades:" + string.Join(",", wifisIntensidadesfixed));
        string macString = string.Join(",",wifisMACRef);
        string intesidadesString = string.Join(",", wifisIntensidadesfixed);
        WWWForm form = new WWWForm();
        form.AddField("macs", macString);
        form.AddField("intensities", intesidadesString);
        // Predecir piso
        StartCoroutine(APIHelper.POST("predict", form, response => {
            pred = JsonUtility.FromJson<Prediccion>(response);
            Debug.Log(pred.prediction);
            npiso.text = "Numero de piso: " + pred.prediction.ToString();
            npiso2.text = "Numero de piso: " + pred.prediction.ToString();
        }));
            yield return new WaitForSeconds(10);
            StartCoroutine(WifiDisponibles());

        }

        }

    public void Start(){
        //DestinosDisponibles();
        StartCoroutine(WifiDisponibles());
    }

    // Agregar a utilities
    public class WifiJson {
        public List<string> macs;
        public List<int> intensities;
        public int floor;
    }
    // Agregar a utilities
    public class Prediccion {
        public List<string> macs;
        public int prediction;
    }

    public void CambiarPiso()
    {
        string piso = inputCambiarPiso.text;
        npiso.text = "Numero de piso: " + piso;
        npiso2.text = "Numero de piso: " + piso;
    }
    // Agregar a utilites
    
}