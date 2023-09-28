using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedFormatoWifi : MonoBehaviour
{
    /*
     * Funcion que homogeniza las redes wifi, para que en cada lista se tengan las mismas redes wifi y se pueda calcular la altura
     * (Mover donde se utiliza)
    */
    public List<int> generarVector(List<string> macBD, List<string> macAndroid , List<int> intesidadAndroid)
    {
        /*
         macDB => Mac reconocidas en la DB
         macAndroid => Macs detectadas por el celular
         intensidadAndroid => valores de las intensidades de android
         */
        List<int> intensidadesFixed = new List<int>();
        for (int i = 0; i < macBD.Count; i++)
        {
            intensidadesFixed.Add(0);
        }

        for (int i = 0; i < macBD.Count; i++) //Se iteran las macDB
        {
            //si existe una macAndroid  del mismo nombre, se agrega su intensidad
            //de lo contrario se coloca un 0
            if (macAndroid.Contains(macBD[i]))
            {
                int a = macAndroid.IndexOf(macBD[i]);
                intensidadesFixed[i] = intesidadAndroid[a];
            }
        }
        
        return intensidadesFixed;
    }
}
