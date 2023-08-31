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
    /*
    [SerializeField]
    public TMP_InputField xInput;
    [SerializeField]
    public TMP_InputField yInput;
    */
    [SerializeField]
    public TMP_InputField nameInput;
    [SerializeField]
    public TMP_InputField pisoDatasetInput; //Input de piso para guardar las redes wifi en la DB
    /*
    [SerializeField]
    public TextMeshProUGUI posicionTexto;*/
    [SerializeField]
    TMP_InputField descripcionInput;

    [SerializeField]
    public MyPositionGPS GPS_handler;
    [SerializeField]
    GameObject prefabNearby;
    [SerializeField]
    GameObject NearbyScroll;

    public List<Point> _pointlist = new List<Point>();
    public Prediccion pred;

    // --------------------------------------- //
    static int SortByMac(WifiData w1, WifiData w2){
        return w1.mac.CompareTo(w2.mac);
    }

    static int SortByFloor(WifiData w1, WifiData w2){
        return w1.floor.CompareTo(w2.floor);
    }

    IEnumerator SendPoints(int floor, string macs, string intensities)
    {
        const string IP = "144.22.42.236";
        //const string IP = "localhost";
        const string port = "3000";
        const string baseURI = "http://"+IP+":"+port+"/api/";
        // Crear formulario
        WWWForm form = new WWWForm();
        form.AddField("macs", macs);
        form.AddField("intensities", intensities);
        form.AddField("floor", floor);
        //Realizar request
        UnityWebRequest www = UnityWebRequest.Post(baseURI+"beta/add", form);
        yield return www.SendWebRequest();
        // Resolucion de la request
        if (www.result != UnityWebRequest.Result.Success)
        {
            
            Debug.Log("Error post: "+ www.error);
            _ShowAndroidToastMessage("Error al enviar los datos");
        }
        else
        {
            Debug.Log("Form upload complete!");
            _ShowAndroidToastMessage("Datos guardados");
        }
    }

    // Guardar datos para generar dataset de puntos wifi asociados a un piso
    public void GenerarDataset()
    {
        int piso = int.Parse(pisoDatasetInput.text);
        string macs = string.Join(",", wifisMAC);
        string intensidades = string.Join(",", wifisIntensidades);
        StartCoroutine(SendPoints(piso, macs, intensidades));
    }

    IEnumerator guardarPunto()
    {
        Debug.Log("Guardando punto...");
        const string IP = "144.22.42.236";
        //const string IP = "localhost";
        const string port = "3000";
        const string baseURI = "http://" + IP + ":" + port + "/api/";
        // Crear formulario
        WWWForm form = new WWWForm();


        //Debug.Log(xInput.text + yInput.text + pisoInput.text + tipoInput.text + nameInput.text);

        _ShowAndroidToastMessage("Guardando punto...");

        string xPos = GPS_handler.GetLastPosition()[0].ToString();
        string yPos = GPS_handler.GetLastPosition()[1].ToString();

        //posicionTexto.text = xPos + ',' + yPos;

        // ============= Request para guardar el punto ==============
        form.AddField("x", xPos);
        form.AddField("y", yPos);
        form.AddField("floor", pisoInput.text);
        form.AddField("tipo", tipoInput.text);
        form.AddField("name", nameInput.text);
        form.AddField("description", descripcionInput.text);
        Debug.Log("Enviando solicitud");
        //Realizar request
        UnityWebRequest www = UnityWebRequest.Post(baseURI+"points/add", form);
        yield return www.SendWebRequest();
        // Resolucion de la request
        if (www.result != UnityWebRequest.Result.Success)
        {
            //_ShowAndroidToastMessage("Error: "+ www.error);
            Debug.Log("Error post: "+ www.error);
        }
        else
        {
            _ShowAndroidToastMessage("Punto guardado");
            Debug.Log("Form upload complete!");
        }
        // ============= Request para agregar los vecinos ==============
        GameObject[] toggles = GameObject.FindGameObjectsWithTag("NearbyPoint");

        List<string> vecinos = new List<string>();

        Debug.Log("Toggles: "+toggles.Length);
        foreach (GameObject toggle in toggles)
        {
            if (toggle.GetComponent<Toggle>().isOn)
            {
                string vecino = toggle.GetComponentInChildren<Text>().text;
                vecinos.Add(vecino);
                Debug.Log("Vecinos: " + vecino);
                //_ShowAndroidToastMessage(vecino);
            }
        }

        // Solo guardar vecinos si realmente hay vecinos
        if(vecinos.Count > 0)
        {
            string _vecinos = string.Join(",", vecinos);
            //_ShowAndroidToastMessage(_vecinos);
            WWWForm form2 = new WWWForm();
            form2.AddField("origen", nameInput.text);
            form2.AddField("vecinos", _vecinos);
            _ShowAndroidToastMessage("Guardando vecinos");
            //Realizar request
            UnityWebRequest www2 = UnityWebRequest.Post(baseURI + "points/addArc", form2);
            yield return www2.SendWebRequest();
            // Resolucion de la request
            if (www2.result != UnityWebRequest.Result.Success)
            {
                _ShowAndroidToastMessage("Error" + www2.error);
                Debug.Log("Error post: " + www2.error);
            }
            else
            {
                _ShowAndroidToastMessage("Vecinos guardados");
                Debug.Log("Form upload complete!");
            }
        }

    }

    IEnumerator actualizarPunto(string name, string nombreAntiguo, string description, string tipo, string vecinos, float x, float y, int piso)
    {
        // ============== Codigo para borrar punto ===============
        const string IP = "144.22.42.236";
        //const string IP = "localhost";
        const string port = "3000";
        const string baseURI = "http://" + IP + ":" + port + "/api/";
        // Crear formulario
        WWWForm form = new WWWForm();
        //_ShowAndroidToastMessage("Actualizando punto...");
        //Realizar request
        UnityWebRequest www = UnityWebRequest.Post(baseURI + "points/" + nombreAntiguo + "/delete", form);
        yield return www.SendWebRequest();
        // Resolucion de la request
        if (www.result != UnityWebRequest.Result.Success)
        {
            //_ShowAndroidToastMessage("Error" + www.error);
            Debug.Log("Error post: " + www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
        }
        // ============== Codigo para agregar punto ===============
        // Crear formulario
        WWWForm form2 = new WWWForm();

        //Debug.Log(xInput.text + yInput.text + pisoInput.text + tipoInput.text + nameInput.text);
        //_ShowAndroidToastMessage("Guardando nuevos datos...");
        //_ShowAndroidToastMessage("Guardando punto...");
        Debug.Log("Creando punto");

        //posicionTexto.text = x.ToString() + ',' + y.ToString();
        Debug.Log(x);
        // ============= Request para guardar el punto ==============
        form2.AddField("x", x.ToString().Replace(",","."));
        form2.AddField("y", y.ToString().Replace(",", "."));
        form2.AddField("floor", piso.ToString());
        form2.AddField("tipo", tipo);
        form2.AddField("name", name);
        form2.AddField("description", description);

        //Realizar request
        UnityWebRequest www2 = UnityWebRequest.Post(baseURI + "points/add", form2);
        yield return www2.SendWebRequest();
        // Resolucion de la request
        if (www2.result != UnityWebRequest.Result.Success)
        {
            _ShowAndroidToastMessage("Error" + www2.error);
            Debug.Log("Error post: " + www2.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
        }
        Debug.Log("Vecinos: "+vecinos);
        _ShowAndroidToastMessage(vecinos);
        WWWForm form3 = new WWWForm();
        form3.AddField("origen", name);
        form3.AddField("vecinos", vecinos);
        _ShowAndroidToastMessage("Guardando vecino");
        //Realizar request
        Debug.Log("colocando arcos");
        _ShowAndroidToastMessage("Actualizando vecinos...");
        UnityWebRequest www3 = UnityWebRequest.Post(baseURI + "points/addArc", form3);
        yield return www3.SendWebRequest();
        // Resolucion de la request
        if (www2.result != UnityWebRequest.Result.Success)
        {
            _ShowAndroidToastMessage("Error" + www3.error);
            Debug.Log("Error post: " + www3.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
            _ShowAndroidToastMessage("Actualizacion completa...");
        }
    }

    public void ActualizarPuntoDB(string name, string nombreAntiguo, string description, string tipo, string vecinos, float x, float y, int piso)
    {
        StartCoroutine(actualizarPunto(name, nombreAntiguo, description, tipo, vecinos, x, y, piso));
    }

    IEnumerator nearbyPoints()
    {
        List<Point> auxList = new List<Point>();
        const string IP = "144.22.42.236";
        //const string IP = "localhost";
        const string port = "3000";
        const string baseURI = "http://" + IP + ":" + port + "/api/";

        // Obtner ubicacion actual

        //string xPos = GPS_handler.GetLastPosition()[0].ToString();
        //string yPos = GPS_handler.GetLastPosition()[1].ToString();

        string xPos = "-33.03479";
        string yPos = "-71.59643";

        // Crear formulario
        WWWForm form = new WWWForm();
        // TODO: Reemplazar por xPos e yPos cuando sea un entorno real
        form.AddField("x", xPos);
        form.AddField("y", yPos);

        //_ShowAndroidToastMessage("Buscando cercanos");
        Debug.Log("Buscando cercanos");
        // Pedir listado de puntos cercanos
        UnityWebRequest www = UnityWebRequest.Post(baseURI + "points/nearby", form);
        yield return www.SendWebRequest();
        // Resolucion de la request
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error post: " + www.error);
        }
        else
        {
            Debug.Log("Llenando lista");
            List<string> points = listJson(www.downloadHandler.text);
            for (int i = 0; i < points.Count; i++)
            {
                Point point = JsonUtility.FromJson<Point>(points[i]);
                auxList.Add(point);
                /*
                GameObject instancia = Instantiate(prefabNearby, Vector3.zero, Quaternion.identity);
                instancia.transform.SetParent(NearbyScroll.transform);
                instancia.transform.localScale = Vector3.one;
                instancia.transform.position = Vector3.zero;
                instancia.GetComponentInChildren<Text>().text = point.name;
                */
            }
            _pointlist = auxList;
        }
    }

    IEnumerator nearbyPointsList()
    {
        const string IP = "144.22.42.236";
        //const string IP = "localhost";
        const string port = "3000";
        const string baseURI = "http://" + IP + ":" + port + "/api/";

        // Obtner ubicacion actual

        //string xPos = GPS_handler.GetLastPosition()[0].ToString();
        //string yPos = GPS_handler.GetLastPosition()[1].ToString();

        string xPos = "-33.03479";
        string yPos = "-71.59643";

        // Crear formulario
        WWWForm form = new WWWForm();
        // TODO: Reemplazar por xPos e yPos cuando sea un entorno real
        form.AddField("x", xPos);
        form.AddField("y", yPos);

        //_ShowAndroidToastMessage("Buscando cercanos");
        Debug.Log("Buscando cercanos");
        // Pedir listado de puntos cercanos
        UnityWebRequest www = UnityWebRequest.Post(baseURI + "points/nearby", form);
        yield return www.SendWebRequest();
        // Resolucion de la request
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error post: " + www.error);
        }
        else
        {
            Debug.Log("Llenando lista");
            List<string> points = listJson(www.downloadHandler.text);
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
        }
    }

    public void NearbyPointsListAPI()
    {
        StartCoroutine(nearbyPointsList());
    }
    public void NearbyPointsAPI()
    {
        StartCoroutine(nearbyPoints());
    }

    IEnumerator borrarPunto()
    {
        const string IP = "144.22.42.236";
        //const string IP = "localhost";
        const string port = "3000";
        const string baseURI = "http://"+IP+":"+port+"/api/";
        // Crear formulario
        WWWForm form = new WWWForm();
        Debug.Log(nameInput.text);
        _ShowAndroidToastMessage("Borrando punto...");
        //Realizar request
        UnityWebRequest www = UnityWebRequest.Post(baseURI+"points/"+nameInput.text+"/delete", form);
        yield return www.SendWebRequest();
        // Resolucion de la request
        if (www.result != UnityWebRequest.Result.Success)
        {
            _ShowAndroidToastMessage("Error: " + www.error);
            Debug.Log("Error post: "+ www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
            _ShowAndroidToastMessage("Punto borrado");
        }
    }

    public void GuardarPuntoDB(){
        StartCoroutine(guardarPunto());
    }

    public void BorrarPuntoDB(){
        StartCoroutine(borrarPunto());
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
    IEnumerator FindPointData(string name){
        const string IP = "144.22.42.236";
        //const string IP = "localhost";
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
                if(point.tipo != "especial"){
                    GameObject texto = Instantiate(Text, Vector3.zero, Quaternion.identity);
                    texto.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = point.name;
                    texto.transform.parent = contenido.transform;
                    texto.transform.localPosition = Vector3.zero;
                    texto.transform.localScale = Vector3.one;
                }
            }
        }
    }

    IEnumerator WifiDisponibles(){
        const string IP = "144.22.42.236";
        //const string IP = "localhost";
        const string port = "3000";
        const string baseURI = "http://"+IP+":"+port+"/api/";

        UnityWebRequest www =  UnityWebRequest.Get(baseURI+"wifi");
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
            List<WifiJson> points = new List<WifiJson>();
            foreach(string dato in data){
                points.Add(JsonUtility.FromJson<WifiJson>(dato));
            }
            // Entregar resultados
            foreach(string mac in points[0].macs)
            {
                wifisMACRef.Add(mac);
                /*
                GameObject texto = Instantiate(Text, Vector3.zero, Quaternion.identity);
                texto.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = point.name;
                texto.transform.parent = contenido.transform;
                texto.transform.localPosition = Vector3.zero;
                texto.transform.localScale = Vector3.one;*/
            }
        }
        /* wifisIntensidadesfixed = fixedWifi.generarVector(wifisMACRef, wifisMAC,wifisIntensidades);
        print("Se han guardado "+wifisMACRef.Count+ " Macs y identidades: "+wifisIntensidadesfixed.Count); */
        wifisIntensidadesfixed = fixedWifi.generarVector(wifisMACRef, wifisMAC,wifisIntensidades);
        // npiso.text = "we bac";
        // print("Se han guardado "+wifisMACRef.Count+ " Macs y identidades: "+wifisIntensidadesfixed.Count);
        //string macString = "1A:6B:8C:3F:5D:9E,2A:6B:8C:3F:5D:9E,3A:6B:8C:3F:5D:9E,4A:6B:8C:3F:5D:9E";
        //string intesidadesString = "2,3,3,0";
        string macString = string.Join(",",wifisMACRef);
        string intesidadesString = string.Join(",", wifisIntensidadesfixed);
        WWWForm form = new WWWForm();
        form.AddField("macs", macString);
        form.AddField("intensities", intesidadesString);
        www = UnityWebRequest.Post(baseURI+"predict",form);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            _ShowAndroidToastMessage("Error: " + www.error);
            Debug.Log("Error post: "+ www.error);
            //myInputField = CreateInputField();
            //myButton = CreateButton();
        }
        else
        {
            pred = JsonUtility.FromJson<Prediccion>(www.downloadHandler.text);
            Debug.Log(pred.prediction);
            npiso.text= "Numero de piso: " + pred.prediction.ToString();
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

                wifisMAC.Add(wifi.mac);
                wifisIntensidades.Add(wifi.intensity);

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
        StartCoroutine(WifiDisponibles());
    }



    public class WifiJson {
        public List<string> macs;
        public List<int> intensities;
        public int floor;
    }

    public class Prediccion {
        public List<string> macs;
        public int prediction;
    }

    InputField CreateInputField()
    {
        GameObject canvas = GameObject.Find("Canvas");

        GameObject inputFieldGO = new GameObject("InputField");
        inputFieldGO.transform.SetParent(canvas.transform);

        RectTransform rectTransform_inputfield = inputFieldGO.AddComponent<RectTransform>();
        rectTransform_inputfield.anchoredPosition3D = new Vector3(0f,100f,0f);
        rectTransform_inputfield.sizeDelta = new Vector3(100f,30f,0f);
        rectTransform_inputfield.localScale = new Vector3(2.94985f,2.94985f,2.94985f);

        Image image = inputFieldGO.AddComponent<Image>();
        // Personaliza el color de fondo del Input Field
        image.color = Color.gray;

        InputField inputField = inputFieldGO.AddComponent<InputField>();
        inputField.lineType = InputField.LineType.MultiLineNewline;
        inputField.contentType = InputField.ContentType.IntegerNumber;
        
        // Crear un objeto de texto para mostrar en el Input Field
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(inputFieldGO.transform);

        RectTransform rectTransform_text = textGO.AddComponent<RectTransform>();
        rectTransform_text.anchoredPosition3D = new Vector3(0f,0f,0f);
        rectTransform_text.sizeDelta = new Vector3(100f,30f,0f);
        rectTransform_text.localScale = new Vector3(1f, 1f, 1f);
        
        Text text = textGO.AddComponent<Text>();
        text.text = "Indique el piso en el que se encuentra";
        text.color = Color.black;
        text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        text.fontSize = 10;
        
        inputField.textComponent = text;
        return inputField;
    }

    Button CreateButton()
    {
        GameObject canvas = GameObject.Find("Canvas");

        // Crear un objeto para el botón
        GameObject buttonGO = new GameObject("Button_API");
        buttonGO.transform.SetParent(canvas.transform);

        // Configurar el RectTransform
        RectTransform rectTransform = buttonGO.AddComponent<RectTransform>();
        rectTransform.anchoredPosition3D = new Vector3(0f,0f,0f);
        rectTransform.sizeDelta = GameObject.Find("Button").GetComponent<RectTransform>().sizeDelta;
        rectTransform.localScale = GameObject.Find("Button").GetComponent<RectTransform>().localScale;
        //texto.anchoredPosition = button1.GetComponent<RectTransform>().scale;

        // Añadir y configurar el Image component (requerido para el Button component)
        Image image = buttonGO.AddComponent<Image>();
        image.color = new Color(0.0f, 0.47f, 1.0f); // haz el botón verde

        // Añadir y configurar el Button component
        Button button = buttonGO.AddComponent<Button>();
        
        // Crear un objeto para el texto del botón
        GameObject buttonTextGO = new GameObject("ButtonText");
        buttonTextGO.transform.SetParent(buttonGO.transform);
        
        // Configurar el RectTransform del texto
        RectTransform buttonTextRectTransform = buttonTextGO.AddComponent<RectTransform>();
        buttonTextRectTransform.anchoredPosition3D = new Vector3(0f,0f,0f);
        buttonTextRectTransform.sizeDelta = GameObject.Find("Button").GetComponent<RectTransform>().sizeDelta;
        buttonTextRectTransform.localScale = new Vector3(1f,1f,1f);
        
        // Añadir y configurar el Text component
        Text buttonText = buttonTextGO.AddComponent<Text>();
        buttonText.text = "Aceptar";
        buttonText.color = Color.black;
        buttonText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        buttonText.fontSize = 24;
        buttonText.alignment = TextAnchor.MiddleCenter;
        buttonText.fontStyle = FontStyle.Bold;
        
        // Ajustar el Text component al Button
        button.targetGraphic = buttonText;
        
        return button;
    }

    // Agrega este método para manejar el click del botón
    public void OnButtonClick()
    {
        if (int.TryParse(myInputField.text, out int parsedValue))
        {
            inputValue = parsedValue;
            ResetInputField();

            // Actualizamos el texto del displayText con el valor ingresado
            npiso.text = "Numero de piso: " + inputValue.ToString();
        }
        else
        {
            Debug.LogError("El valor ingresado no es un número entero.");
        }
    }

    public void ResetInputField()
    {
        myInputField.text = "";
    }
    public void Update(){
        
        if (myButton != null && myInputField != null)
        {
            myButton.onClick.AddListener(OnButtonClick);
        }
    }

    private void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, message, 0);
                toastObject.Call("show");
            }));
        }
    }

    /*
    public class Point
    {
        public string name;
        public string tipo;
        public float x;
        public float y;
        public int floor;
    }
    */
}