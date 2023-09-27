using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialBehaviour : MonoBehaviour
{

    public GameObject message;
    public TextMeshProUGUI textmessage;
    public List<string> dialogos;
    public List<string> cancelMessage;
    public int n;

    nValue nval;
    /*
     * Numero tutorial
     * 0 = Todos los tutoriales
     * 1 = Seleccionar caminos
     * 2 = Puntos de interés
     * 3 = Sugerencias
     * 4 = Reportes
     */

    int ndialog;
    public bool dialoguing=false;
    int nfinal = -1;
    void Awake()
    {
        nval = GameObject.Find("Data").GetComponent<nValue>();
        n = nval.n;
        print("Data de n = " + n);
        message = GameObject.Find("Mensaje");
        textmessage = GameObject.Find("TituloMensaje").GetComponent<TextMeshProUGUI>();
        ndialog =0;
    }

    // Update is called once per frame
    void Update()
    {

        if (dialoguing && ndialog >= nfinal)
        {
            message.SetActive(false);
            textmessage.text = "";
            dialoguing = false;
        }
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    ndialog++;
                    if(ndialog >= nfinal)
                    {
                        break;
                    }
                    textmessage.text = dialogos[ndialog];
                    break;

            }
        }


    }
    public void enviarDialogo(List<string> dialogos2) //Agregar GameObject(Message), 
    {
        dialogos = dialogos2;
        textmessage.text = dialogos[0];
        message.SetActive(true);
        ndialog = 0;
        int final = dialogos.Count;
        nfinal = final;
        dialoguing = true;
    }
    public void notCanceling() //Agregar GameObject(Message), 
    {
        dialogos = cancelMessage;
        textmessage.text = dialogos[0];
        message.SetActive(true);
        ndialog = 0;
        int final = dialogos.Count;
        nfinal = final;
        dialoguing = true;
    }
    public void tutorialbehaviour(int number)
    {
        Destroy(GameObject.Find("Data"));
        n = number;
    }
}
