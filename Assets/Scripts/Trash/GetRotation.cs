using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetRotation : MonoBehaviour
{
    // Update is called once per frame
    [SerializeField]
    GameObject reference;
    void Update()
    {
        gameObject.transform.rotation = reference.transform.rotation;
    }
}
