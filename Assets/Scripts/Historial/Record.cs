using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Record : MonoBehaviour
{
    List<string> userFrequentRequests;
    [SerializeField]
    HistorialBehaviour hist;
    string userMail;

    // Start is called before the first frame update
    void Start()
    {
        userFrequentRequests = new List<string>();
        userMail = hist.correoVal;
       // listShow();
    }

    /*public void listShow()
    {
        StartCoroutine(GetFrequentUserRequests(userMail, userFrequentRequests));
    }

    public void SendUserRequests(string userMail, string request)
    {
        //-------------------------------------------
        string connectionString = "mongodb://localhost:27017/";
        client = new MongoClient(connectionString);
        database = client.GetDatabase();
        collection = database.GetCollection<BsonDocument>("Frecuencia");
        //-------------------------------------------
        var document = new BsonDocument
        {
            { "mail", userMail },
            { "request", request },
            { "timestamp", DateTime.Now }
        };
        collection.InsertOne(document);

    }


    IEnumerator GetFrequentUserRequests(string userMail, List<string> frequentRequests)
    {
        //-------------------------------------------
        string connectionString = "mongodb://localhost:27017/";
        client = new MongoClient(connectionString);
        database = client.GetDatabase();
        collection = database.GetCollection<BsonDocument>("Frecuencia");
        //-------------------------------------------

        var aggregatePipeline = new List<BsonDocument>
        {
            BsonDocument.Parse("{ $match: { correo: '" + userMail + "' } }"),
            BsonDocument.Parse("{ $group: { _id: '$request', count: { $sum: 1 } } }"),
            BsonDocument.Parse("{ $sort: { count: -1 } }"),
            BsonDocument.Parse("{ $limit: 3 }")
        };
        var cursor = collection.Aggregate<BsonDocument>(aggregatePipeline).ToList();

        frequentRequests.Clear();
        foreach (var result in cursor)
        {
            string frecuentrequest = result["_id"].AsString;
            frequentRequests.Add(frecuentrequest);
        }
    }

    /*private void DeleteUserRequests(string userMail, string request)
    {
        //-------------------------------------------
        string connectionString = "mongodb://localhost:27017/";
        client = new MongoClient(connectionString);
        database = client.GetDatabase();
        collection = database.GetCollection<BsonDocument>("Frecuencia");
        //-------------------------------------------
        try
        {
            var filtro = Builders<BsonDocument>.Filter.And(
            Builders<BsonDocument>.Filter.Eq("correo", userMail),
            Builders<BsonDocument>.Filter.Eq("request", request)
            );
            var resultado = collection.DeleteMany(filtro);
        }
        catch ()
        {
            Debug.Log("error");
        }
    }

    private void DeleteAllUserRequests(string userMail, string request)
    {
        //-------------------------------------------
        string connectionString = "mongodb://localhost:27017/";
        client = new MongoClient(connectionString);
        database = client.GetDatabase();
        collection = database.GetCollection<BsonDocument>("Frecuencia");
        //-------------------------------------------
        try
        {
            var filtro = Builders<BsonDocument>.Filter.Eq("mail", userMail);
            var resultado = collection.DeleteMany(filtro);
        }
        catch ()
        {
            Debug.Log("error");
        }
    }*/
}
