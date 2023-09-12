using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class APIHelper
{
    const string _IP = "144.22.42.236";
    const string _PORT = "3000";
    const string base_uri = "http://" + _IP + ":" + _PORT + "/api/";

    public static IEnumerator POST(string recurso, WWWForm form, System.Action<string> onComplete)
    {
        UnityWebRequest www = UnityWebRequest.Post(base_uri + recurso, form);
        yield return www.SendWebRequest();
        if(www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            Utilities._ShowAndroidToastMessage("Error: " + www.error);
        } 
        else
        {
            onComplete(www.downloadHandler.text);
        }
    }
    public static IEnumerator POST(string recurso, WWWForm form, System.Action<string> onComplete, System.Action<string> onError)
    {
        UnityWebRequest www = UnityWebRequest.Post(base_uri + recurso, form);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            onError(www.error);
        }
        else
        {
            onComplete(www.downloadHandler.text);
        }
    }

    public static IEnumerator POST(string recurso, WWWForm form)
    {
        UnityWebRequest www = UnityWebRequest.Post(base_uri + recurso, form);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            Utilities._ShowAndroidToastMessage("Error: " + www.error);
        }
        else
        {
            Debug.Log("Form uploaded!");
        }
    }

    public static IEnumerator GET(string recurso, System.Action<string> onComplete)
    {
        UnityWebRequest www = UnityWebRequest.Get(base_uri + recurso);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            Utilities._ShowAndroidToastMessage("Error: " + www.error);
        }
        else
        {
            onComplete(www.downloadHandler.text);
        }
    }



}
