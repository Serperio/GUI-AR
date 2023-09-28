using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Threading.Tasks;
using UnityEngine.Android;
using UnityEngine.UI;
using TMPro;

public class Register : MonoBehaviour
{

    [SerializeField]
    TMP_InputField emailGet;
    [SerializeField]
    TMP_InputField passGet;


    [SerializeField]
    UIBehaviour ui;

    [SerializeField]
    string email; //Campo Email

    [SerializeField]
    string password; //Campo Contrasena

    [SerializeField]
    string Text;

    [System.Serializable]
    public class ApiResponse
    {
        public string message;
        public Userreg user;
    }

    public class Userreg
    {
        public int Id;
        public string Email;
        public string Password;
        public bool isAdmin;
        public bool IsLoggedIn;
    }
    private void Update()
    {
        email = emailGet.text;
        password = passGet.text;
        if (status)
        {
            //Cargar escena tutorial
        }
    }

    const string IP = "144.22.42.236";
    const string PORT = "3000";
    
    const string api_url1 = "http://" + IP + ":" + PORT + "/api/login";
    const string api_url2 = "http://" + IP + ":" + PORT + "/api/users";

    public bool status = false;

    // Update is called once per frame
    public void RegisterUser()
    {
        StartCoroutine(LoginSendData());
    }
    IEnumerator LoginSendData()
    {
        WWWForm json = new WWWForm();
        json.AddField("mail", email);
        json.AddField("password", password);
        ApiResponse response = null;

        UnityWebRequest request = UnityWebRequest.Post(api_url1, json);

        //request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        //if (request.result == UnityWebRequest.Result.Success)
        //{
            string response1 = request.downloadHandler.text;
            Debug.Log("Log response:" + request.downloadHandler.text);

            response = JsonUtility.FromJson<ApiResponse>(response1);

            Debug.Log("Log response:" + request.downloadHandler.text);

            if (response.message == "User not found.")
            {
                UnityWebRequest request2 = UnityWebRequest.Post(api_url2, json);

                //request2.SetRequestHeader("Content-Type", "application/json");
                yield return request2.SendWebRequest();

                if (request2.result == UnityWebRequest.Result.Success)
                {
                    Text = "Registro exitoso";
                    status = true;

                }
                else
                {
                    //error solicitud
                    Debug.LogError("Error " + request.error);
                    Text = "Ha ocurrido un error de conexión. Inténtelo más tarde";
                }
            }
            else
            {
                Text = "Usuario ya registrado";
            }
            
        //}
        /*else
        {
            //error solicitud
            Debug.LogError("Error en la solicitud: " + request.error);
            status = false;
            Text = "Ha ocurrido un error de conexión. Inténtelo más tarde";
        }*/
    }
}

