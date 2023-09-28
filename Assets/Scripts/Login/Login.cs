using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Threading.Tasks;
using UnityEngine.Android;
using UnityEngine.UI;

public class Login : MonoBehaviour
{

    [SerializeField]
    string email; //Campo Email

    [SerializeField]
    string password; //Campo Contrasena

    [SerializeField]
    string Text;

    [System.Serializable]
    public class ApiResponse
    {
        public string response;
        public Userlog user;
    }

    public class Userlog
    {
        public int Id;
        public string Email;
        public string Password;
        public bool isAdmin;
        public bool IsLoggedIn;
    }


    const string IP = "localhost";
    const string PORT = "3000";
    const string api_url = "http://" + IP + ":" + PORT + "/api/";

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
        ApiResponse response = null;

        UnityWebRequest request = UnityWebRequest.Post(api_url + "login", json.ToString());
 
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string response1 = request.downloadHandler.text;
            Debug.Log("Log response:" + request.downloadHandler.text);

            response = JsonUtility.FromJson<ApiResponse>(response1);

            Debug.Log("Log response:"+ request.downloadHandler.text);

            if (response.response == "User not found")
            {
                status = false;
                Text = "Correo o contrase�a inv�lidos";
            }
            else
            {
                if (response.response == "User Logged In")
                {
                    status = true;
                    Text = "Correo o contrase�a inv�lidos";
                }
                else
                {
                    status = false;
                    Text = "Correo o contrase�a inv�lidos";
                }
            }

        }
        else
        {
            // Hubo un error en la solicitud
            Debug.LogError("Error en la solicitud: " + request.error);
            status = false;
            Text = "Ha ocurrido un error de conexi�n. Int�ntelo m�s tarde";
        }
    }

}
