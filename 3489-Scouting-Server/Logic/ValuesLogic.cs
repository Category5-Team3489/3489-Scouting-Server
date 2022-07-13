namespace ScoutingServer3489.Logic;

public class ValuesLogic
{
    private readonly ScoutingServer server;

    public ValuesLogic(ScoutingServer server)
    {
        this.server = server;
    }

    public async Task<bool> TryLoadExisting(string field, Action<string> loader)
    {
        var jsonCursor = await server.Mongo.Values.FindAsync(Builders<BsonDocument>.Filter.Exists(field));
        var jsonDocument = await jsonCursor.FirstOrDefaultAsync();
        if (jsonDocument is not null)
        {
            var eventMatchesJson = jsonDocument.GetValue(field).AsString;
            loader(eventMatchesJson);
            Console.WriteLine($"loaded previous {field}");
            return true;
        }
        return false;
    }

    public async Task<bool> TryLoadPreviousEventKey()
    {
        var eventKeyCursor = await server.Mongo.Values.FindAsync(Builders<BsonDocument>.Filter.Exists("event_key"));
        var eventMatchesJsonDocument = await eventMatchesJsonCursor.FirstOrDefaultAsync();
        if (eventMatchesJsonDocument is not null)
        {
            var eventMatchesJson = eventMatchesJsonDocument.GetValue("event_matches_json").AsString;
            Tba.LoadEventMatches(eventMatchesJson);
            Console.WriteLine("loaded previous event matches json");
        }
    }
}