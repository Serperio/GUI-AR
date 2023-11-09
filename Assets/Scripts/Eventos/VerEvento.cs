using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

public class VerEvento : MonoBehaviour
{

    [SerializeField]
    public GameObject panel_descripcion_evento;
    [SerializeField]
    public GameObject preFab;

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
        const string IP = "144.22.42.236";
        //const string IP = "localhost";
        const string port = "3000";
        const string baseURI = "http://" + IP + ":" + port + "/api/";

        UnityWebRequest www = UnityWebRequest.Get(baseURI + "eventos?all=true");
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
                GameObject cada_evento = Instantiate(preFab, Vector3.zero, Quaternion.identity);

                //GameObject gameObject1 = GameObject.Find("AppManager"); //Realizar los correspondientes flips con el boton aceptar.
/*                 UIBehaviour script1 = gameObject1.GetComponent<UIBehaviour>();
                script1.FlipUI(canva_filtros); */

                TextMeshProUGUI tituloText = cada_evento.GetComponentInChildren<TextMeshProUGUI>();
                Button botoncito = cada_evento.GetComponentInChildren<Button>();

                Image miImagen = cada_evento.AddComponent<Image>();
                tituloText.text = evento.nombre;
                System.DateTime parseDate = System.DateTime.Parse(evento.fecha_inicio);
                botoncito.onClick.AddListener(delegate{MostrarDescripcion(evento.nombre, evento.descripcion, evento.img, parseDate.ToString("dd-MM-yyyy"), System.DateTime.Parse(evento.fecha_termino).ToString("dd-MM-yyyy"));});

                StartCoroutine(CargarImagen(evento.img, cada_evento));
                cada_evento.transform.parent = contenido.transform;
                cada_evento.transform.localPosition = Vector3.zero;
                cada_evento.transform.localScale = Vector3.one;
                //Debug.Log("Parte 2 \n");
                //Debug.Log("Nombre:"+evento.nombre+"\ndescripcion: "+evento.descripcion+"\nURL_img: "+evento.img+"\nFecha: +"+parseDate.ToString("dd-MM-yyyy"));
            }
        }
    }


    IEnumerator CargarImagen(string url, GameObject objeto)

    {
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
        eventos.Sort((a, b) =>
        {
            System.DateTime inicio_a = System.DateTime.Parse(a.fecha_inicio);
            System.DateTime inicio_b = System.DateTime.Parse(b.fecha_inicio);
            return inicio_a.CompareTo(inicio_b);
        });
        // forma 2
        //eventos.OrderBy(x => x.fecha)
        return eventos;
    }

    public void BuscarEventosExistentes(){
        StartCoroutine(EventosExistentes());
    }

    public void MostrarDescripcion(string nombre, string descripcion, string img_url, string fecha, string fecha_termino)
    {
        GameObject gameObject1 = GameObject.Find("AppManager");
        UIBehaviour scriptUI = gameObject1.GetComponent<UIBehaviour>();
        scriptUI.FlipUI(panel_descripcion_evento);

        TextMeshProUGUI Titulo = GameObject.Find("NombreEvento").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI Descripcion = GameObject.Find("DescripcionEvento").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI Fecha = GameObject.Find("FechaEvento").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI FechaTermino = GameObject.Find("FechaTerminoEvento").GetComponent<TextMeshProUGUI>();

        StartCoroutine(CargarImagen(img_url, GameObject.Find("ImgEvento")));
        Titulo.text = nombre;
        Descripcion.text = descripcion;
        //Fecha.text = fecha.ToString("yyyy/MM/dd");
        
        Fecha.text = fecha;
        FechaTermino.text = fecha_termino;
    }
}
