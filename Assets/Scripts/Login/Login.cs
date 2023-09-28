using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Threading.Tasks;
using UnityEngine.Android;
using UnityEngine.UI;
using TMPro;

public class Login : MonoBehaviour
{
    [SerializeField]
    TMP_InputField emailGet;
    [SerializeField]
    TMP_InputField passGet;
    [SerializeField]
    string email; //Campo Email
    [SerializeField]
    string password; //Campo Contrasena
    [SerializeField]
    string Text;

    [SerializeField]
    UIBehaviour ui;

    [System.Serializable]
    public class ApiResponse
    {
        public string message;
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

    const string IP = "144.22.42.236";
    const string PORT = "3000";
    const string api_url = "http://" + IP + ":" + PORT + "/api/";

    public bool status = false;

    // Update is called once per frame
    public void logInApp()
    {
        StartCoroutine(LoginSendData());
    }
    private void Update()
    {
        email = emailGet.text;
        password = passGet.text;
        if (status)
        {
            //Cargar escena tutorial
            ui.LoaderScenes(1);
        }
    }
    IEnumerator LoginSendData()
    {
        WWWForm json = new WWWForm();
        json.AddField("mail", email);
        json.AddField("password", password);
        ApiResponse response = null;

        UnityWebRequest request = UnityWebRequest.Post(api_url + "login", json);

        //request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        //if (request.result == UnityWebRequest.Result.Success)
        //{
            string response1 = request.downloadHandler.text;
            Debug.Log("Log response:" + request.downloadHandler.text);

            response = JsonUtility.FromJson<ApiResponse>(response1);

            Debug.Log("Log response:"+ request.downloadHandler.text);

            if (response.message == "User not found")
            {
                status = false;
                Text = "Correo o contraseña inválidos";
            }
            else
            {
                if (response.message == "User Logged In")
                {
                    status = true;
                    Text = "";
                }
                else
                {
                    status = false;
                    Text = "Correo o contraseña inválido";
                }
            }

        //}
        //else
        //{
            // Hubo un error en la solicitud
        //    Debug.LogError("Error en la solicitud: " + request.error);
        //    status = false;
        //    Text = "Ha ocurrido un error de conexión. Inténtelo más tarde";
        //}
    }

}
