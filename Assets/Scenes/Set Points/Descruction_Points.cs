using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Descruction_Points : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("MainCamera"))
        {
        Destroy(gameObject);
        }
    }

   
}