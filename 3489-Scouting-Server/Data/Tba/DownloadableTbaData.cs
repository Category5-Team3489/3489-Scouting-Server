namespace ScoutingServer3489.Data.Tba;

public class DownloadableTbaData<T> where T : ?
{
    private readonly HttpClient httpClient;
    private readonly Func<string, string> endpointProvider;

    private string? eventKey = null;

    public T? Data { get; private set; } = null;
    public string? DataJson { get; private set; } = null;

    public DownloadableTbaData(HttpClient httpClient, Func<string, string> endpointProvider)
    {
        this.httpClient = httpClient;
        this.endpointProvider = endpointProvider;
    }
}