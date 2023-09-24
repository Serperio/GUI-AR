using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptBotonMas : MonoBehaviour
{
    public VerEvento verEvento;
    public GameObject VistaDescripcion;

    void Start(){
        //VistaDescripcion = gameObject.GetComponentInChildren<GameObject>();
        VistaDescripcion = GameObject.Find("PanelDescripcionEvento");
        verEvento = GameObject.Find("AppManager").GetComponent<VerEvento>();
    }

    public void Detalle(){
        verEvento.VerDetalle();
    }
}
