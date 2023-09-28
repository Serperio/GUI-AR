using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Register : MonoBehaviour
{
    [SerializeField]
    GameObject email; //Campo Email

    [SerializeField]
    GameObject password; //Campo Contrasena

    [SerializeField]
    GameObject Text;


    const string IP = ;
    const string PORT = ;
    const string api_url = "http://" + IP + ":" + PORT + "/api/";

    public bool status = false;

    // Update is called once per frame
    void Start()
    {
        StartCoroutine(LoginSendData());
    }

    public async void LoginSendData()
    {
        JSONObject json = new JSONObject();
        json.AddField("mail", email);
        json.AddField("password", password);

        UnityWebRequest request = UnityWebRequest.Post(apiUrl + "login", json.ToString());

        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {

            string responseText = request.downloadHandler.text;
            Debug.Log("Respuesta de la API: " + responseText);

            if (responseText != "User not found")
            {
                UnityWebRequest request = UnityWebRequest.Post(apiUrl + "users", json.ToString());

                request.SetRequestHeader("Content-Type", "application/json");
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {

                    string responseText = request.downloadHandler.text;
                    Debug.Log("Respuesta de la API: " + responseText);

                    if (responseText != "User not found")
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
