using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;


[System.Serializable]
public class DescriptionItem
{
    public string imageURL;
    public string description;
}

public class Info_Rutas : MonoBehaviour
{
    /*
     Buscador para encontrar algun destino con nombre
    */

    [SerializeField]
    GameObject contenido; //Donde se mostraran los datos
    [SerializeField]
    GameObject Text; //Campo del Input
    [SerializeField]
    TextMeshProUGUI Ubi_actual;
    [SerializeField]
    GameObject textoPunto; //Texto donde se escribe la informacion
    [SerializeField]
    private List<Info_Point> infopoint_especifica;

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
    /*
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
                    texto.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = point.name;
                    texto.transform.parent = contenido.transform;
                    texto.transform.localPosition = Vector3.zero;
                    texto.transform.localScale = Vector3.one;
                    Debug.Log(point.tipo);
                }
            }
        }
    }
    */

    /* public void GetInput(TMP_InputField s)
     { //Obtener valor input
         inputText = s.text;
         StartCoroutine(FindPointData(inputText));
     }*/
    
    private void Start()
    {
        Ubi_actual.text = "Cañon";
        //InvokeRepeating("UpdatePointData", 0f, 10f);
        UpdatePointData();

    }

    private void UpdatePointData()
    {
        StartCoroutine(FindPointData(Ubi_actual.text));
    }

    IEnumerator FindPointData(string name) //Buscar los datos de un punto por nombre
    {
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
                //textoPunto.GetComponentInChildren<TextMeshProUGUI>().text = "Descripción: \n" +"\n"+ point.description;
                string idpunto = point._id;
                StartCoroutine(FindPointInfo(idpunto));



            }
            catch
            {
                OpenDetalle();
                textoPunto.GetComponentInChildren<TextMeshProUGUI>().text = "Destino no valido.";
            }

        }
        //StartCoroutine(FindPointData(name));
    }

    //Dsps Cambiar a UIbehaviour, (comportamiento de botones)

    /*
    public void BuscarSitiosDisponibles()
    {
        StartCoroutine(DestinosDisponibles());
    }
    */


    IEnumerator FindPointInfo(string point_id) //Buscar los datos de un punto por nombre
    {
        Debug.Log("ID punto: " + point_id);
        //const string IP = "144.22.42.236";
        const string IP = "localhost";
        const string port = "3000";
        const string baseURI = "http://" + IP + ":" + port + "/api/";
        WWWForm form = new WWWForm();
        form.AddField("ID_Point", point_id);
        UnityWebRequest www = UnityWebRequest.Post(baseURI + "info_points/find", form);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error post: " + www.error);
        }
        else
        {
            try
            {
                Debug.Log("Entre al primero222");
                // Recuperar JSON
                string response = www.downloadHandler.text;
                //Point Infopoint = JsonUtility.FromJson<Point>(response);
                // Obtener listado de puntos
                Debug.Log("estoy aqui 1");
                Debug.Log(response);

                List<string> pointlist_total = JsonToList(response);
                // Transformar JSON a Point
                infopoint_especifica = new List<Info_Point>();
                Debug.Log("estoy aqui 2");
                Debug.Log(pointlist_total.Count);
                infopoint_especifica.Clear();
                foreach (string info in pointlist_total)
                {
                    Info_Point point = JsonUtility.FromJson<Info_Point>(info);
                    if (point.ID_Point == point_id)
                    {
                        Debug.Log("point: " + point.imagen);
                        infopoint_especifica.Add(point);
                    }
                }
                foreach (Info_Point point in infopoint_especifica)
                {
                    if((point.imagen == "") || (point.imagen == null)){

                    }
                    Debug.Log("url:" + point.imagen + "\ndescripcion: " + point.descripcion + "\nidPOINT: " + point.ID_Point);
                    Debug.Log("entre al segundo for");
                    GameObject cada_point = Instantiate(Text, Vector3.zero, Quaternion.identity);
                    TextMeshProUGUI descripcion = cada_point.GetComponentInChildren<TextMeshProUGUI>();
                    //Image miImagen = cada_point.GetComponentInChildren<Image>();
                    Debug.Log(descripcion);
                    StartCoroutine(CargarImagen(point.imagen, cada_point));
                    descripcion.text = point.descripcion;

                    cada_point.transform.parent = contenido.transform;
                    cada_point.transform.localPosition = Vector3.zero;
                    cada_point.transform.localScale = Vector3.one;
                    //Debug.Log("Parte 2 \n");
                }
            }
            catch
            {
                Debug.Log("Entre al segundo");
                OpenDetalle();
                textoPunto.GetComponentInChildren<TextMeshProUGUI>().text = "El destino no tiene puntos de interés.";
            }

        }
        //StartCoroutine(FindPointData(name));
    }

    IEnumerator CargarImagen(string url, GameObject objeto /*,GameObject loadingMessage*/)
    {
        /*   if (loadingMessage != null)
           {
               loadingMessage.SetActive(true);
           }*/
        GameObject not_image = GameObject.Find("NotImage");
        not_image.SetActive(false);
        if (objeto == null)
        {
            // El objeto ha sido destruido, no hacemos nada aquí.
            yield break;
        }

        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();

            // Oculta el mensaje de "cargando" después de que se complete la descarga
           /* if (loadingMessage != null)
            {
                loadingMessage.SetActive(false);
            }*/

            if (objeto == null)
            {
                // El objeto ha sido destruido mientras esperábamos la respuesta.
                yield break;
            }

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Image imagen = objeto.GetComponentInChildren<Image>();
                imagen.enabled = false;
                not_image.SetActive(true);
                Debug.LogError("Error al cargar la imagen desde la URL: " + www.error);
            }
            else
            {
                // cargamos la textura y obtenemos componente imagen
                Texture2D textura = ((DownloadHandlerTexture)www.downloadHandler).texture;

                if (textura != null)
                {
                    // Asegúrate de que el objeto todavía existe antes de asignar el sprite
                    if (objeto != null)
                    {
                        not_image.SetActive(false);
                        Image imagen = objeto.GetComponentInChildren<Image>();

                        if (imagen != null)
                        {
                            // Crea un sprite a partir de la textura
                            Sprite sprite = Sprite.Create(textura, new Rect(0, 0, textura.width, textura.height), Vector2.zero);
                            // Asigna el sprite a la imagen
                            imagen.sprite = sprite;
                        }
                    }
                }
            }
        }
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
