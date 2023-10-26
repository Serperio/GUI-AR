using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Record : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        listShow();
    }

    public void listShow()
    {

        StartCoroutine(GetFrequentUserRequests(userEmail, userFrequentRequests));

        //-------------------------------------------------------
        GameObject buttonTemplate = transform.GetChild(0).gameObject;
        GameObject g;

        N = searchHistory.Length; //3?

        for (int i = 0; i < 3; i++)
        {
            g = Instantiate(buttonTemplate, transform);

            g.transform.GetChild(1).GetComponent<Text>().text = searchHistory[i];

            g.GetComponent<Button>().AddEventListener(i, Buscador.FindPointData(name));
            g.GetComponent<Button>().AddEventListener(i, DeleteRecord(i));
        }

        Destroy(buttonTemplate);
    }

    // Update is called once per frame
    void Update()
    {

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


    private void GetFrequentUserRequests(string userMail, List<string> frequentRequests)
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

    private void DeleteUserRequests(string userMail, string request)
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
            Builders<BsonDocument>.Filter.Eq( "request", request )
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
    }
}
