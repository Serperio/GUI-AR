using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimacionesUI : MonoBehaviour
{
    [SerializeField] private GameObject menuExtra;

    public bool menuActivado = false;
    // Start is called before the first frame update


/*     private void BajarAlpha()
    {
        LeanTween.alpha(inicioGrupo.GetComponent<RectTransform>(), 0f, 1f).setDelay(0.5f);
        inicioGrupo.GetComponent<CanvasGroup>().blocksRaycasts = false;
    } */

    public void ActivarMenuExtra()
    {
        if(!menuActivado)
        {
            menuActivado = true;
            LeanTween.moveY(menuExtra.GetComponent<RectTransform>(),-200,1f).setEase(LeanTweenType.easeOutCirc);
        }
        else
        {
            menuActivado = false;
            LeanTween.moveY(menuExtra.GetComponent<RectTransform>(),805,1f).setEase(LeanTweenType.easeOutCirc);
        }

    }

    public void DesactivarMenuExtra(){
        if(menuActivado){
            LeanTween.moveY(menuExtra.GetComponent<RectTransform>(),805,1f).setEase(LeanTweenType.easeOutCirc);
        }
        menuActivado = false;
    }
}
