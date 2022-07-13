namespace ScoutingServer3489.Data;

public class TbaData
{
    private readonly HttpClient httpClient;
    private readonly string apiKey;

    private string? eventKey = null;

    private string? matchesEventKey = null;
    private string? teamsEventKey = null;

    public MatchSimpleSchema[]? EventMatches { get; private set; } = null;
    public string? EventMatchesJson { get; private set; } = null;
    public TeamSimpleSchema[]? EventTeams { get; private set; } = null;
    public string? EventTeamsJson { get; private set; } = null;

    public TbaData(HttpClient httpClient, string apiKey)
    {
        this.httpClient = httpClient;
        this.apiKey = apiKey;
    }

    public void UpdateEventKey(string eventKey)
    {
        this.eventKey = eventKey;

        if (string.IsNullOrEmpty(matchesEventKey) || matchesEventKey != eventKey)
        {
            EventMatches = null;
            EventMatchesJson = null;
        }

        if (string.IsNullOrEmpty(teamsEventKey) || teamsEventKey != eventKey)
        {
            EventTeams = null;
            EventTeamsJson = null;
        }
    }

    public void LoadEventMatches(string json)
    {
        EventMatches = JsonSerializer.Deserialize<MatchSimpleSchema[]>(json);
        EventMatchesJson = json;
    }
    public void LoadEventTeams(string json)
    {
        EventTeams = JsonSerializer.Deserialize<TeamSimpleSchema[]>(json);
        EventTeamsJson = json;
    }

    public async Task UpdateEventMatches()
    {
        if (string.IsNullOrEmpty(eventKey))
        {
            return;
        }

        matchesEventKey = eventKey;

        try
        {
            string json = await httpClient.GetStringAsync($"https://www.thebluealliance.com/api/v3/event/{eventKey}/matches/simple?X-TBA-Auth-Key={apiKey}");
            LoadEventMatches(json);
        }
        catch (Exception)
        {
            EventMatches = null;
            EventMatchesJson = null;
        }
    }
    public async Task UpdateEventTeams()
    {
        if (string.IsNullOrEmpty(eventKey))
        {
            return;
        }

        teamsEventKey = eventKey;

        try
        {
            string json = await httpClient.GetStringAsync($"https://www.thebluealliance.com/api/v3/event/{eventKey}/teams/simple?X-TBA-Auth-Key={apiKey}");
            LoadEventTeams(json);
        }
        catch (Exception)
        {
            EventTeams = null;
            EventTeamsJson = null;
        }
    }
}