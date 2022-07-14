namespace ScoutingServer3489.Data;

public class TbaData
{
    private readonly HttpClient httpClient;
    private readonly MongoData mongo;
    private readonly string apiKey;

    public LoadableTbaData<MatchSimpleSchema[]> EventMatchesData { get; private set; }
    public LoadableTbaData<TeamSimpleSchema[]> EventTeamsData { get; private set; }

    public TbaData(HttpClient httpClient, MongoData mongo, string apiKey)
    {
        this.httpClient = httpClient;
        this.mongo = mongo;
        this.apiKey = apiKey;

        EventMatchesData = new(
            httpClient,
            async () => await DbUtils.TryGetCollectionField(mongo.Values, "event_matches_json"),
            async json => await DbUtils.UpsertCollectionField(mongo.Values, "event_matches_json", json),
            async () => await DbUtils.DeleteCollectionFields(mongo.Values, "event_matches_json"),
            eventKey => $"https://www.thebluealliance.com/api/v3/event/{eventKey}/matches/simple?X-TBA-Auth-Key={apiKey}"
        );
        EventTeamsData = new(
            httpClient,
            async () => await DbUtils.TryGetCollectionField(mongo.Values, "event_teams_json"),
            async json => await DbUtils.UpsertCollectionField(mongo.Values, "event_teams_json", json),
            async () => await DbUtils.DeleteCollectionFields(mongo.Values, "event_teams_json"),
            eventKey => $"https://www.thebluealliance.com/api/v3/event/{eventKey}/teams/simple?X-TBA-Auth-Key={apiKey}"
        );
    }

    public async Task UpdateEventKeyAndLoadData(string eventKey)
    {
        await EventMatchesData.UpdateEventKey(eventKey);
        if (await EventMatchesData.TryLoadData())
        {
            Console.WriteLine("loaded event matches");
        }
        else
        {
            Console.WriteLine("unable to load event matches");
        }

        await EventTeamsData.UpdateEventKey(eventKey);
        if (await EventTeamsData.TryLoadData())
        {
            Console.WriteLine("loaded event teams");
        }
        else
        {
            Console.WriteLine("unable to load event teams");
        }
    }
}