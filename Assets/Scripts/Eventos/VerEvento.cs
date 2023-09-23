using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

public class VerEvento : MonoBehaviour
{

    public Evento events;
    [SerializeField]
    public GameObject Text;

    [SerializeField]
    public GameObject contenido;

    // Start is called before the first frame update

    private void Start()
    {
        BuscarEventosExistentes();
    }


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

    IEnumerator EventosExistentes()

    {
        //const string IP = "144.22.42.236";
        const string IP = "localhost";
        const string port = "3000";
        const string baseURI = "http://" + IP + ":" + port + "/api/";

        UnityWebRequest www = UnityWebRequest.Get(baseURI + "evento");
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error post: " + www.error);
        }
        else
        {
            // Recuperar JSON
            string response = www.downloadHandler.text;
            // Obtener listado de Eventos
            List<string> data = listJson(response);
            List<Evento> eventos = new List<Evento>();
            foreach(string i in data)
            {
                eventos.Add(JsonUtility.FromJson<Evento>(i));
            }

            eventos = OrdenarEventos(eventos);

            foreach(Evento evento in eventos)
            {
                GameObject cada_evento = Instantiate(Text, Vector3.zero, Quaternion.identity);

                TextMeshProUGUI tituloText = cada_evento.GetComponentInChildren<TextMeshProUGUI>();

                Image miImagen = cada_evento.AddComponent<Image>();
                tituloText.text = evento.nombre;

                StartCoroutine(CargarImagen(evento.img, cada_evento));
                cada_evento.transform.parent = contenido.transform;
                cada_evento.transform.localPosition = Vector3.zero;
                cada_evento.transform.localScale = Vector3.one;
                //Debug.Log("Parte 2 \n");
                Debug.Log("Nombre:"+evento.nombre+"\ndescripcion: "+evento.descripcion+"\nURL_img: "+evento.img);
            }
        }
    }


    IEnumerator CargarImagen(string url, GameObject objeto)

    {

        if (objeto != null)
        {
            Debug.Log("VACIOOOOOOOOOO CONCHETUMARE");
        } 
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error al cargar la imagen desde la URL: " + www.error);
            }
            else
            {
                // cargamos la textura y obtenemos componente imagen
                Texture2D textura = ((DownloadHandlerTexture)www.downloadHandler).texture;
                Image imagen = objeto.GetComponent<Image>();

                if (textura != null)
                {
                    // Crea un sprite a partir de la textura
                    Sprite sprite = Sprite.Create(textura, new Rect(0, 0, textura.width, textura.height), Vector2.zero);

                    // Asigna el sprite a la imagen
                    imagen.sprite = sprite;
                }
            }
        }
    }

    static List<Evento> OrdenarEventos(List<Evento> eventos)
    {
        // forma 1
        // si fuera ascendente, usar <= en vez de =>.
        eventos.Sort((a, b) => a.fecha.CompareTo(b.fecha));
        // forma 2
        //eventos.OrderBy(x => x.fecha)
        return eventos;
    }

    public void BuscarEventosExistentes(){
        StartCoroutine(EventosExistentes());
    }
}
