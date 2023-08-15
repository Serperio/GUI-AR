using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prueba : MonoBehaviour
{
    [SerializeField]
    Valores valor;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(valor.b);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
