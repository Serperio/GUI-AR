using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{

    [SerializeField]
    GameObject email; //Campo Email

    [SerializeField]
    GameObject password; //Campo Contrasena

    [SerializeField]
    GameObject Text;


    const string IP = "localhost";
    const string PORT = "3000";
    const string base_uri = "http://" + IP + ":" + PORT + "/api/login";

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
             
        UnityWebRequest request = UnityWebRequest.Post(apiUrl, json.ToString());
 
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            
            string responseText = request.downloadHandler.text;
            Debug.Log("Respuesta de la API: " + responseText);

            if (responseText == "User not found")
            {
                status = false;
                Text = "Correo o contraseña inválidos";
            }
            else
            {
                if (responseText == "User Logged In")
                {
                    status = true;
                    Text = "Correo o contraseña inválidos";
                }
                else (responseText == "Wrong Password")
                {
                    status = false;
                    Text = "Correo o contraseña inválidos";
                }
            }

        }
        else
        {
            // Hubo un error en la solicitud
            Debug.LogError("Error en la solicitud: " + request.error);
            status = false;
            Text = "Ha\r\nocurrido un error de conexión. Inténtelo más tarde";
        }
    }

}

public class User
{
    public ObjectId Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public bool isAdmin { get; set; }
    public bool IsLoggedIn { get; set; }
}