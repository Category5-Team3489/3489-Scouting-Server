namespace ScoutingServer3489.Data.Tba;

public class LoadableTbaData<T> where T : class
{
    private readonly HttpClient httpClient;
    private readonly Func<Task<string?>> getData;
    private readonly Func<string, Task> upsertData;
    private readonly Func<string, string> endpointProvider;

    private string? eventKey = null;

    public T? Data { get; private set; } = null;
    public string? Json { get; private set; } = null;

    public LoadableTbaData(HttpClient httpClient, Func<Task<string?>> getData, Func<string, Task> upsertData, Func<string, string> endpointProvider)
    {
        this.httpClient = httpClient;
        this.getData = getData;
        this.upsertData = upsertData;
        this.endpointProvider = endpointProvider;
    }

    // true if event key was updated
    public bool UpdateEventKey(string eventKey)
    {
        if (this.eventKey != eventKey)
        {
            Data = null;
            Json = null;
            return true;
        }
        return false;
    }

    // true if data was successfully loaded
    public async Task<bool> TryLoadData()
    {
        var json = await getData();
        if (json is not null)
        {
            return await TryDeserializeJson(json);
        }
        return false;
    }

    // true if data was successfully downloaded and loaded
    public async Task<bool> TryDownloadAndLoadData()
    {
        if (string.IsNullOrEmpty(eventKey))
        {
            return false;
        }

        try
        {
            string endpoint = endpointProvider(eventKey);
            string json = await httpClient.GetStringAsync(endpoint);
            return await TryDeserializeJson(json);
        }
        catch (Exception e)
        {
            Data = null;
            Json = null;
            Console.WriteLine($"failed to download and load data, error: {e}");
            return false;
        }
    }

    // true if json was successfully deserialized
    private async Task<bool> TryDeserializeJson(string json)
    {
        try
        {
            if (Json != json)
            {
                Data = JsonSerializer.Deserialize<T>(json);
                Json = json;

                await upsertData(json);
            }
            return true;
        }
        catch (Exception e)
        {
            Data = null;
            Json = null;
            Console.WriteLine($"failed to deserialize data, error: {e}");
            return false;
        }
    }
}