namespace ScoutingServer3489.Utils;

public static class DbUtils
{
    public static async Task<string?> TryGetCollectionField(IMongoCollection<BsonDocument> collection, string field)
    {
        var cursor = await collection.FindAsync(Builders<BsonDocument>.Filter.Exists(field));
        var document = await cursor.FirstOrDefaultAsync();
        if (document is not null)
        {
            return document.GetValue(field).AsString;
        }
        return null;
    }
    public static async Task UpsertCollectionField(IMongoCollection<BsonDocument> collection, string field, string value)
    {
        await collection.UpdateOneAsync(
            Builders<BsonDocument>.Filter
            .Exists(field),
            Builders<BsonDocument>.Update
            .Set(field, value),
            new UpdateOptions() { IsUpsert = true }
        );
    }
    public static async Task DeleteCollectionFields(IMongoCollection<BsonDocument> collection, string field)
    {
        await collection.DeleteManyAsync(Builders<BsonDocument>.Filter.Exists(field));
    }
}