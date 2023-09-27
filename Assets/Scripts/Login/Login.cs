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
    // Start is called before the first frame update
    {
        string connectionString = "mongodb://localhost:27017"; // Reemplaza con tu propia URL de conexión
        client = new MongoClient(connectionString);
        database = client.GetDatabase("miapp"); // Reemplaza con el nombre de tu base de datos
        userCollection = database.GetCollection<User>("usuarios"); // Reemplaza con el nombre de tu colección de usuarios
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async void Login()
    {
    string email = emailInput.text;
    string password = passwordInput.text;

    var filter = Builders<User>.Filter.Eq(u => u.Email, email);
    var user = await userCollection.Find(filter).FirstOrDefaultAsync();

    if (user != null)
    {
        if (user.Password == password)
        {
            if (user.IsLoggedIn)
            {
                loginStatusText.text = "El usuario ya ha iniciado sesión en otro dispositivo.";
            }
            else
            {
                // Marcar al usuario como conectado
                user.IsLoggedIn = true;
                var update = Builders<User>.Update.Set(u => u.IsLoggedIn, true);
                await userCollection.UpdateOneAsync(filter, update);

                loginStatusText.text = "Inicio de sesión exitoso";

                // Aquí puedes agregar lógica para cargar la escena principal o realizar otras acciones después del inicio de sesión.
            }
        }
        else
        {
            loginStatusText.text = "Contraseña incorrecta";
        }
    }
    else
        {
            loginStatusText.text = "Correo electrónico no encontrado";
        }
    }
}

public class User
{
    public ObjectId Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public bool IsLoggedIn { get; set; }
}