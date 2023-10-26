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
    TextMeshProUGUI mensajeee;

    bool isAdmin=false;
    [SerializeField]
    UIBehaviour ui;

    [System.Serializable]
    public class ApiResponse
    {
        public string message;
        public Userlog user;
        public bool admin;
    }

    public class Userlog
    {
        public int _id;
        public string mail;
        public string password;
        public bool isAdmin;
        public bool isLoggedIn;
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
        mensajeee.text = Text;
        email = emailGet.text;
        password = passGet.text;
        if (status)
        {
            GameObject data = GameObject.Find("Data");
            Destroy(data);
            Debug.Log("ADMIN: " + isAdmin);
            //Cargar escena tutorial
            if (isAdmin)
            {
            ui.LoaderScenes(3);
            }
            else
            {
            ui.LoaderScenes(1); //Cambiar a modo usuarioé 
            }
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
                    Text = "";
                    Debug.Log(response.admin);
                    isAdmin=response.admin;
                    status = true;
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
