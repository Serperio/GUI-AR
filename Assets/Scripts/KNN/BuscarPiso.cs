using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BuscarPiso : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI mostrarPiso;

    public void predecirPiso(){
        Debug.Log(KNN.predict(new int[]{4,0,0,0,0,0,0,0,0,0,8,11,10,8,0,0,0,7,0,18,0,7,13,12,0,0,0,0,10,16,0,0,0,0,4,16,17,10,16,5,0,8,0,0,0,0,15,10,0,10,0,0,0,0,13,0,0,0,0,0,7,0,0,0,19,0,0,0,7,6,8,0,0}));
    }
}
