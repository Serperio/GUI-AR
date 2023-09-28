using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Threading.Tasks;
using UnityEngine.Android;
using UnityEngine.UI;

public class Register : MonoBehaviour
{
    [SerializeField]
    string email; //Campo Email

    [SerializeField]
    string password; //Campo Contrasena

    [SerializeField]
    string Text;

    


    const string IP = "localhost";
    const string PORT = "3000";
    const string api_url1 = "http://" + IP + ":" + PORT + "/api/users";
    const string api_url2 = "http://" + IP + ":" + PORT + "/api/login";

    public bool status = false;

    // Update is called once per frame
    void Start()
    {
        StartCoroutine(LoginSendData());
    }

    IEnumerator LoginSendData()
    {
        WWWForm json = new WWWForm();
        json.AddField("mail", email);
        json.AddField("password", password);

        UnityWebRequest request = UnityWebRequest.Post(api_url2, json.ToString());

        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {

            string responseText = request.downloadHandler.text;
            Debug.Log("Respuesta de la API: " + responseText);

            if (responseText != "User not found")
            {
                UnityWebRequest request2 = UnityWebRequest.Post(api_url1, json.ToString());

                request2.SetRequestHeader("Content-Type", "application/json");
                yield return request2.SendWebRequest();

                if (request2.result == UnityWebRequest.Result.Success)
                {

                    string responseText2 = request2.downloadHandler.text;
                    Debug.Log("Respuesta de la API: " + responseText2);

                    if (responseText2 != "User not found")
                    {
                        Text = "Registro exitoso";
                    }

                }
                else
                {
                    //error solicitud
                    Debug.LogError("Error " + request.error);
                    Text = "Ha ocurrido un error de conexión. Inténtelo más tarde";
                }
            }
            
        }
        else
        {
            //error solicitud
            Debug.LogError("Error en la solicitud: " + request.error);
            status = false;
            Text = "Ha ocurrido un error de conexión. Inténtelo más tarde";
        }
    }
}
