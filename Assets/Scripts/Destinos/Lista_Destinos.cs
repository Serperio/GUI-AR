using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;


public class Lista_Destinos : MonoBehaviour
{
    [SerializeField]
    GameObject contenido;
    [SerializeField]
    GameObject Text; //Campo del Input
    public void Start()
    {
        DestinosDisponibles();
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

    void DestinosDisponibles()
    {
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

}
