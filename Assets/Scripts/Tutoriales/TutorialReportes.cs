using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialReportes : MonoBehaviour
{
    bool transicion;
    int paso;
    [SerializeField]
    TutorialBehaviour tutorialBehaviour;
    public List<string> dial0;
    public List<string> dial1;
    public List<string> dial2;
    [SerializeField]
    GameObject thisCanvas;
    [SerializeField]
    UIBehaviour ui;

    // Start is called before the first frame update
    void Start()
    {
        if (tutorialBehaviour.n == 4)
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
                    tutorialBehaviour.enviarDialogo(dial2);
                    paso++;
                    transicion = false;
                    break;
            }
        }
        if (!tutorialBehaviour.dialoguing && paso == 3 && Input.touchCount == 0)
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
