using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedFormatoWifi : MonoBehaviour
{
    public List<int> generarVector(List<string> macBD, List<string> macAndroid , List<int> intesidadAndroid)
    {
        List<int> intensidadesFixed = new List<int>();

        for (int i = 0; i < macBD.Count; i++)
        {
            if (macAndroid.Contains(macBD[i]))
            {
                int a = macAndroid.IndexOf(macBD[i]);
                intensidadesFixed.Add(intesidadAndroid[a]);
            }
            else
            {
                intensidadesFixed.Add(0);
            }
        }
        
        return intensidadesFixed;
    }
}
