using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoutingServer3489.Data;

public class MongoData
{
    private readonly MongoClient dbClient;

    public IMongoDatabase Scouting { get; private set; }
    public IMongoCollection<BsonDocument> Values { get; private set; }

    public MongoData(MongoClient dbClient)
    {
        this.dbClient = dbClient;

        Scouting = dbClient.GetDatabase("scouting");
        Values = Scouting.GetCollection<BsonDocument>("values");
    }

    public void PrintValuesCollection()
    {
        var docs = Values.Find(new BsonDocument());
        Console.WriteLine("values: ");
        foreach (BsonDocument d in docs.ToList())
        {
            Console.WriteLine($"\t{d}");
        }
        Console.WriteLine();
    }

    public void PrintDatabases()
    {
        var dbList = dbClient.ListDatabases().ToList();
        Console.WriteLine("databases: ");
        foreach (var db in dbList)
        {
            Console.WriteLine($"\t{db}");
        }
        Console.WriteLine();
    }
}