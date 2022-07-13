Console.WriteLine("Hello, World!");

/*
Console.WriteLine("--------------------------------");

// /event/2022new/matches/simple
// /event/2022new/teams/simple

HttpClient client = new();
string ep = "/event/2022new/matches/simple";
string response = await client.GetStringAsync($"https://www.thebluealliance.com/api/v3{ep}?X-TBA-Auth-Key={apiKey}");
MatchSimpleSchema[]? schemas = JsonSerializer.Deserialize<MatchSimpleSchema[]>(response);
if (schemas is not null)
{
    foreach (var schema in schemas)
    {
        schema.PrintProperties();
        Console.WriteLine("--------------------------------");
    }
}
*/

var server = new ScoutingServer(IPAddress.Loopback, 8080, await File.ReadAllTextAsync(Directory.GetCurrentDirectory() + @"\key.secret"));
server.Start();

while (true)
{
    if (Console.KeyAvailable)
    {
        ConsoleKey key = Console.ReadKey().Key;
        if (key == ConsoleKey.X)
        {
            break;
        }
    }

    await server.Update();

    await Task.Delay(10);
}