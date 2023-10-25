using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

public class EditDropdown : MonoBehaviour
{

    public TMP_Dropdown dropdownPiso;
    public TMP_Dropdown dropdownEdificio;
    public Button botonAceptarFiltro;
    public GameObject Aviso_FilterVacio;
    public GameObject canva_filtros;
    public Button SinFiltro_Seccion;
    public Button ConFiltro_Seccion;
    public TMP_Text nohaydestinos;
    public GameObject ScrollDestinos;
    [SerializeField]
    GameObject contenido_filter;
    [SerializeField]
    GameObject Text; //Campo del Input (el boton para cada cosa)
    private int? selectedValuePiso; // Variable para almacenar el valor seleccionado del Dropdown de Piso
    private string selectedValueEdificio; // Variable para almacenar el valor seleccionado del Dropdown de Edificio

// ----------------- //

    [SerializeField]
    string puntoFinal;
    [SerializeField]
    API api;
    [SerializeField]
    TextMeshProUGUI mensajeFinal;
    private LocationInfo lastLocation;
    private float lastLatitude;
    private float lastLongitude; 
    System.DateTime currentDateTime;
    MyPositionGPS gpsloc;
    public List<Point> pointList1;
    public List<Point> pointList;
    public string puntoInicial;

// ----------------- //

    private void Start()
    {
        //Debug.Log("iniciooo");
        dropdownPiso.captionText.text = "Seleccione una opci�n";
        dropdownEdificio.captionText.text = "Seleccione una opci�n";
        StartCoroutine(DestinosDisponibles());
        botonAceptarFiltro.onClick.AddListener(EjecutarFiltro);
        // Escucha el evento de cambio de selecci�n de los Dropdowns.
        //dropdownPiso.onValueChanged.AddListener(delegate { OnDropdownValueChanged(dropdownPiso, ref selectedValuePiso); });
        //dropdownEdificio.onValueChanged.AddListener(delegate { OnDropdownValueChanged(dropdownEdificio, ref selectedValueEdificio); });
    }

// ------------------------------------------------------------------------------------------- //
// ------------------------------------------------------------------------------------------- //

// ------------------------------------------------------------------------------------------- //
// ------------------------------------------------------------------------------------------- //


    private void EjecutarFiltro()
    {
        if (((string.IsNullOrEmpty(dropdownPiso.captionText.text)) && string.IsNullOrEmpty(dropdownEdificio.captionText.text)) ||
            ((dropdownPiso.captionText.text == "Seleccione una opci�n") && (dropdownEdificio.captionText.text == "Seleccione una opci�n")) ||
            ((dropdownPiso.captionText.text == "Seleccione una opci�n") && string.IsNullOrEmpty(dropdownEdificio.captionText.text)) ||
            ((string.IsNullOrEmpty(dropdownPiso.captionText.text)) && (dropdownEdificio.captionText.text == "Seleccione una opci�n")))
        {

            Debug.Log("Entre al Null");
            Aviso_FilterVacio.SetActive(true);
        }
        else
        {
            GameObject gameObject1 = GameObject.Find("AppManager"); //Realizar los correspondientes flips con el boton aceptar.
            UIBehaviour script1 = gameObject1.GetComponent<UIBehaviour>();
            script1.FlipUI(canva_filtros);

            //borrar_filtro.SetActive(true);
            if (!string.IsNullOrEmpty(dropdownPiso.captionText.text) && dropdownPiso.captionText.text != "Seleccione una opci�n")
            {
                selectedValuePiso = int.Parse(dropdownPiso.captionText.text); //Pasar el valor del piso de string a int
                Debug.Log("Entre 1: " + selectedValuePiso + " " + selectedValuePiso.GetType());
            }
            else
            {
                selectedValuePiso = null;
            }

            if (!string.IsNullOrEmpty(dropdownEdificio.captionText.text) && dropdownEdificio.captionText.text != "Seleccione una opci�n")
            {
                selectedValueEdificio = dropdownEdificio.captionText.text;
                Debug.Log("Entre 2: " + selectedValueEdificio + " " + selectedValueEdificio.GetType());
            }
            else
            {
                selectedValueEdificio = "Null";
            }

            // Limpia el contenido actual en contenido_filter
            erase_content();

            //Debug.Log("TEST ASIGNACION PISO: " + selectedValuePiso);
            //Debug.Log("TEST ASIGNACION EDIFICIO: " + selectedValueEdificio);

            int? filtroPiso = selectedValuePiso;
            string filtroEdificio = selectedValueEdificio;
            StartCoroutine(DestinosDisponiblesFilter(filtroPiso, filtroEdificio));
        }
    }

    private void erase_content()
    {
        foreach (Transform child in contenido_filter.transform)
        {
            Debug.Log("borrando: " + child.gameObject);
            Destroy(child.gameObject);
        }
    }
    /*
    private void OnDropdownValueChanged(TMP_Dropdown dropdown, ref int selectedValue)
    {
        int index = dropdown.value;
        Debug.Log("Valor seleccionado antes if: " + index);
        // Cuando se selecciona una opci�n en el Dropdown, actualiza el mensaje o placeholder.
        if (index >= 0)
        {
            selectedValue = dropdown.options[index].text;
            dropdownPiso.captionText.text = selectedValue; // Borra el mensaje predeterminado.
            Debug.Log("Valor seleccionado: " + selectedValue);
        }
        else
        {
            Debug.Log("Entre al else");
            selectedValue = null;
        }
    }

    */

    /* private void SetPlaceholderText(TMP_Text textComponent, string text)
     {
         // Funci�n para establecer el mensaje o placeholder de un objeto TextMeshPro.
         textComponent.text = text;
     }*/

    static List<string> listJson(string jsonData)
    {
        string json = jsonData.Substring(1, jsonData.Length - 2);
        bool startParentesis = false;
        bool endParentesis = false;
        string dato = "";
        List<string> strings = new List<string>();
        foreach (char caracter in json)
        {
            if (caracter == '{')
            {
                dato = "";
                startParentesis = true;
            }
            else if (caracter == '}')
            {
                endParentesis = true;
            }
            if (startParentesis)
            {
                dato += caracter;
            }
            if (endParentesis)
            {
                startParentesis = false;
                endParentesis = false;
                strings.Add(dato);
            }
        }
        return strings;
    }
    static List<string> JsonToList(string jsonData)
    { //Convierte Json a Lista
        string json = jsonData.Substring(1, jsonData.Length - 2);
        bool startParentesis = false;
        bool endParentesis = false;
        string dato = "";
        List<string> strings = new List<string>();
        foreach (char caracter in json)
        {
            if (caracter == '{')
            {
                dato = "";
                startParentesis = true;
            }
            else if (caracter == '}')
            {
                endParentesis = true;
            }
            if (startParentesis)
            {
                dato += caracter;
            }
            if (endParentesis)
            {
                startParentesis = false;
                endParentesis = false;
                strings.Add(dato);
            }
        }
        return strings;
    }

    IEnumerator DestinosDisponiblesFilter(int? filtroPiso, string filtroEdificio)
    {
        const string IP = "144.22.42.236";
        //const string IP = "localhost";
        const string port = "3000";
        const string baseURI = "http://" + IP + ":" + port + "/api/";

        UnityWebRequest www = UnityWebRequest.Get(baseURI + "points");
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error post: " + www.error);
        }
        else
        {
            // Recuperar JSON
            string response = www.downloadHandler.text;
            // Obtener listado de puntos
            List<string> data = JsonToList(response);
            // Transformar JSON a Point
            List<Point> points = new List<Point>();
            foreach (string dato in data)
            {
                Point point = JsonUtility.FromJson<Point>(dato);
                // Aplicar filtros
                if (point.tipo != "especial")
                {
                    if (point.floor == filtroPiso && (selectedValueEdificio == "Null"))
                    {
                        points.Add(point);
                    }
                    else if (point.name == filtroEdificio && (selectedValuePiso == null))
                    {
                        points.Add(point);
                    }
                    else if (point.name == filtroEdificio && point.floor == filtroPiso)
                    {
                        points.Add(point);
                    }
                }
            }
            // Entregar resultados
            if (points.Count > 0)
            {
                ScrollDestinos.SetActive(true);
                nohaydestinos.gameObject.SetActive(false);
                foreach (Point point in points)
                {
                    GameObject texto = Instantiate(Text, Vector3.zero, Quaternion.identity);
                    texto.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = point.name;
                    texto.transform.parent = contenido_filter.transform;
                    texto.transform.localPosition = Vector3.zero;
                    texto.transform.localScale = Vector3.one;
                    Debug.Log(point.tipo);
                }
            }
            else
            {
                ScrollDestinos.SetActive(false);
                nohaydestinos.gameObject.SetActive(true);
            }

        }
        ConFiltro_Seccion.gameObject.SetActive(true);
        SinFiltro_Seccion.gameObject.SetActive(false);
    }


    IEnumerator DestinosDisponibles()
    { //Obtiene todos los destinos desde la base de datos
        const string IP = "144.22.42.236";
        //const string IP = "localhost";
        const string port = "3000";
        const string baseURI = "http://" + IP + ":" + port + "/api/";

        UnityWebRequest www = UnityWebRequest.Get(baseURI + "points");
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error post: " + www.error);
        }
        else
        {
            // Recuperar JSON
            string response = www.downloadHandler.text;
            // Obtener listado de puntos
            List<string> data = JsonToList(response);
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
                    //texto.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = point.name;
                    texto.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "WEWEWEWEWEWE";
                    texto.transform.parent = contenido_filter.transform;
                    texto.transform.localPosition = Vector3.zero;
                    texto.transform.localScale = Vector3.one;
                    //texto.onClick.onClick.AddListener(() => ObtenerInfoRuta(point.name));
                    Debug.Log(point.tipo);
                }
            }
            ScrollDestinos.SetActive(true);
            nohaydestinos.gameObject.SetActive(false);
        }
    }
    public void LimpiarFiltros()
    {
        ConFiltro_Seccion.gameObject.SetActive(false);
        SinFiltro_Seccion.gameObject.SetActive(true);
        erase_content();
        StartCoroutine(DestinosDisponibles());
        dropdownPiso.value = 0; // Establecer el valor seleccionado del dropdownPiso en -1 (ninguna selecci�n)
        dropdownEdificio.value = 0;
        dropdownPiso.RefreshShownValue(); // Actualizar el valor mostrado en el dropdown
        dropdownEdificio.RefreshShownValue(); // Actualizar el valor mostrado en el dropdown
    }

}