using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDestinos : MonoBehaviour
{
    bool transicion;
    int paso;
    [SerializeField]
    TutorialBehaviour tutorialBehaviour;
    public List<string> dial0;
    public List<string> dial1;
    public List<string> dial2;
    public List<string> dial3;
    public List<string> dial4;
    public List<string> dial5;
    public List<string> dial6;
    [SerializeField]
    GameObject botonDestino;
    [SerializeField]
    GameObject botonDestinoOff;
    [SerializeField]
    GameObject botonVerRuta;
    [SerializeField]
    GameObject botonVerRutaOff;
    [SerializeField]
    GameObject botonCambiarPiso;
    [SerializeField]
    GameObject botonCambiarPisoOff;
    [SerializeField]
    UIBehaviour ui;

    bool atime=true;
    [SerializeField]
    TutorialInfo tutorialInfo;


    [SerializeField]
    GameObject thisCanvas;
    [SerializeField]
    GameObject infoCanvas;



    // Start is called before the first frame update
    void Start()
    {
        if (tutorialBehaviour.n<=1)
        {
            transicion = true;
        }
        paso = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (transicion){
            switch (paso)
            {
                case 0:
                    thisCanvas.SetActive(true);
                    tutorialBehaviour.enviarDialogo(dial0);
                    paso++;
                    transicion = false;
                    break;
                case 1:
                    tutorialBehaviour.enviarDialogo(dial1);
                    paso++;
                    transicion = false;
                    break;
                case 2:
                    botonDestino.SetActive(false);
                    botonDestinoOff.SetActive(true);
                    botonVerRuta.SetActive(true);
                    botonVerRutaOff.SetActive(false);
                    tutorialBehaviour.enviarDialogo(dial2);
                    paso++;
                    transicion = false;
                    break;
                case 3:
                    tutorialBehaviour.enviarDialogo(dial3);
                    paso++;
                    transicion = false;
                    break;
                case 4:
                    botonVerRuta.SetActive(false);
                    botonVerRutaOff.SetActive(true);
                    botonCambiarPiso.SetActive(true);
                    botonCambiarPisoOff.SetActive(false);
                    tutorialBehaviour.enviarDialogo(dial4);
                    paso++;
                    transicion = false;
                    break;
                case 5:
                    tutorialBehaviour.enviarDialogo(dial5);
                    paso++;
                    transicion = false;
                    break;
                case 6:
                    tutorialBehaviour.enviarDialogo(dial6);
                    paso++;
                    transicion = false;
                    break;
            }
        }
        if (!tutorialBehaviour.dialoguing && paso == 7 && tutorialBehaviour.n==0&&atime)
        {
            atime = false;
            tutorialInfo.nextStep();
            thisCanvas.SetActive(false);
            infoCanvas.SetActive(true);
        }
        else if (!tutorialBehaviour.dialoguing && paso == 7 && tutorialBehaviour.n != 0 && Input.touchCount==0)
        {
            tutorialBehaviour.n = 10;
            Destroy(GameObject.Find("Data"));
            ui.LoaderScenes(0);
        }
    }
    public void nextStep()
    {
        transicion = true;
    }
}
