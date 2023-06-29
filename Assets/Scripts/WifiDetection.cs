using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WifiDetection : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI csm;
    [SerializeField]
    TextMeshProUGUI wifiOn;

    // Start is called before the first frame update
    void FixedUpdate()
    {
        LocationService service = new LocationService();
        csm.text = service.isEnabledByUser ? "Si se ha activado el GPS" : "No ha sido activado el GPS";
        print(Application.internetReachability);
        wifiOn.text = Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork ? "Si se ha activado el Wifi" : "No se ha activado el Wifi";
    }

    // Update is called once per frame
}
