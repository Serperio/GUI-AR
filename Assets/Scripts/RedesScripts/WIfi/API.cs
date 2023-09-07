using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

public class API : MonoBehaviour
{
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
    TMP_InputField descripcionInput;

    [SerializeField]
    public MyPositionGPS GPS_handler; // Deberia ser singleton
    [SerializeField]
    GameObject prefabNearby;
    [SerializeField]
    GameObject NearbyScroll;

    public List<Point> _pointlist = new List<Point>();
    public Prediccion pred;

    // Guardar datos para generar dataset de puntos wifi
    //  asociados a un piso en la coleccion beta
    public void GenerarDataset()
    {
        int piso = int.Parse(pisoDatasetInput.text);
        string macs = string.Join(",", wifisMAC);
        string intensidades = string.Join(",", wifisIntensidades);

        WWWForm form = new WWWForm();
        form.AddField("macs", macs);
        form.AddField("intensities", intensidades);
        form.AddField("floor", piso);
        //Realizar request
        StartCoroutine(APIHelper.POST("beta/add", form));
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
        form2.AddField("x", x.ToString().Replace(",","."));
        form2.AddField("y", y.ToString().Replace(",", "."));
        form2.AddField("floor", piso.ToString());
        form2.AddField("tipo", tipo);
        form2.AddField("name", name);
        form2.AddField("description", description);
        yield return StartCoroutine(APIHelper.POST("points/add", form2));
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
        StartCoroutine(APIHelper.POST("points/" + borrarInput.text + "/delete", form));
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

    void DestinosDisponibles(){
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

    IEnumerator WifiDisponibles(){
        // Obtener redes wifi conocidas
        yield return StartCoroutine(APIHelper.GET("wifi", response => {
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
        wifisIntensidadesfixed = fixedWifi.generarVector(wifisMACRef, wifisMAC,wifisIntensidades);
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
    }

    public void Start(){
        DestinosDisponibles();
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