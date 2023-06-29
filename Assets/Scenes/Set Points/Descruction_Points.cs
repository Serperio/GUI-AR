using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Descruction_Points : MonoBehaviour
{
    GameObject flecha;
    private void Start()
    {
        flecha = GameObject.Find("Arrow");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("MainCamera"))
        {
        flecha.GetComponent<ArrowRotation>().DeletePoint();
        Destroy(gameObject);
        }
    }

   
}