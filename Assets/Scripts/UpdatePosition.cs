using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatePosition : MonoBehaviour
{
    GameObject cambiador;
    //Vector3 PosicionInicial;
    // Start is called before the first frame update
    void Start()
    {
        cambiador = GameObject.Find("Arrow");
    }
    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = cambiador.GetComponent<GetLocation>().GetUserLocation();
    }
}
