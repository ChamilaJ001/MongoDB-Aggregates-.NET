using MongoDB.Bson;
using MongoDB.Driver;

MongoClient client  = new MongoClient("mongodb+srv://chamila:chamila123@cluster0.j48yjr3.mongodb.net/myFirstDatabase?retryWrites=true&w=majority");

IMongoCollection<BsonDocument> playListCollection = client.GetDatabase("sample_mflix").GetCollection<BsonDocument>("playlist");

List<BsonDocument> results = playListCollection.Find(new BsonDocument()).ToList();  

foreach (BsonDocument result in results)
{
    Console.WriteLine(result["username"] + ": " + string.Join(", ", result["items"]));
}

List<BsonDocument> pResults = playListCollection.Aggregate()
    .Match(new BsonDocument { { "username", "nraboy"} })
    .Project(new BsonDocument
        {
            {"_id", 1},
            {"username", 1 },
            {
                "items", new BsonDocument{
                {
                    "$map", new BsonDocument
                    {
                        {"input", "$items"},
                        {"as", "item" },
                        {
                            "in", new BsonDocument
                            {
                                {
                                    "$convert", new BsonDocument
                                    {
                                        {"input", "$$item" },
                                        {"to","objectId" }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            }
        })
    .Lookup("movies", "items", "_id", "movies")
    .Project(new BsonDocument
    {
        {"_id", 1 },
        {"username", 1 },
        {"movies", 1 }
    })
    .ToList();



//BsonDocument piplineStage3 = new BsonDocument
//{
//    {
//        "$lookup", new BsonDocument
//        {
//            {"from", "movies" },
//            {"localField", "items" },
//            {"foreingField", "_id" },
//            {"as","movies" }
//        }
//    }
//};



//BsonDocument[] pipline = new BsonDocument[]{piplineStage3};

//List<BsonDocument> pResults = playListCollection.Aggregate<BsonDocument>(pipline).ToList();

foreach (BsonDocument pResult in pResults)
{
    Console.WriteLine(pResult);
}