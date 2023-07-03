using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

public class MostrarDestinos : MonoBehaviour
{
    [SerializeField]
    API api;
    [SerializeField]
    string IP = "146.235.246.2";
    [SerializeField]
    string port = "3000";

    [SerializeField]
    TextMeshProUGUI destinos;
    [SerializeField]
    ArrowRotation arrowRotation;
    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    private GameObject ruta_point;

    private List<Vector3> puntitos_2;

    void Start() 
    {
        Puntito A = new Puntito("A", -33.04373, -71.63103);
        Puntito B = new Puntito("B", -33.04372, -71.63104);
        Puntito C = new Puntito("C", -33.04372, -71.63105);
        Puntito D = new Puntito("D", -33.04371, -71.63105);

        List<Puntito> vertices = new List<Puntito> { A, B, C, D};

        foreach (Puntito point in vertices)
        {
            if (vertices.Count > 0)
            {
                ruta_point = Instantiate(prefab, new Vector3((float)point.latitud*0.01f, (float)point.longitud*0.01f, 0f), Quaternion.identity);
            }
        }
        VisualizarCamino(vertices);
    }

    void VisualizarCamino(List<Puntito> puntos)
    {
        foreach (Puntito point in puntos)
        {
            Vector3 nuevoVector3 = new Vector3((float)point.latitud*0.01f, (float)point.longitud*0.01f, 0f);
            puntitos_2.Add(nuevoVector3);
        }
        // Crear un nuevo objeto vacío para visualizar el camino
        GameObject caminoObject = new GameObject("Camino");
        
        // Agregar el componente LineRenderer al objeto
        LineRenderer lineRenderer = caminoObject.AddComponent<LineRenderer>();
        
        // Establecer los puntos del camino en el componente LineRenderer
        lineRenderer.positionCount = puntos.Count;
        lineRenderer.SetPositions(puntitos_2.ToArray());
        
        // Establecer otros parámetros del LineRenderer según tus necesidades
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        // ...
    }

}
