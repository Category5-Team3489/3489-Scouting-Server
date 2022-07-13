namespace ScoutingServer3489;

public class ScoutingServer : TcpServer
{
    public TbaData Tba { get; private set; }
    public MongoData Mongo { get; private set; }

    public ValuesLogic Values { get; private set; }

    public SynchronizingQueue<(Guid sender, Message message)> ReceivedMessageQueue { get; private set; }
    public SynchronizingQueue<(bool isDisconnecting, ScoutingSession session)> UpdatedScoutingSessions { get; private set; }

    private readonly Dictionary<string, MessageHandler> messageHandlers = new();
    private readonly Dictionary<Guid, ScoutingSession> scoutingSessions = new();

    private bool isInit = false;

    public ScoutingServer(IPAddress address, int port, string apiKey) : base(address, port)
    {
        // Initial Assignment
        Tba = new(new HttpClient(), apiKey);
        Console.WriteLine("connecting to mongodb");
        Mongo = new(new MongoClient("mongodb://localhost:27017"));
        Console.WriteLine("connected to mongodb");

        Values = new(this);

        ReceivedMessageQueue = new(ReceivedMessage);
        UpdatedScoutingSessions = new(UpdatedScoutingSession);

        // Init
        AddMessageHandlers();

        Mongo.PrintDatabases();
    }

    private void UpdatedScoutingSession((bool isDisconnecting, ScoutingSession session) updatedScoutingSession)
    {
        ScoutingSession session = updatedScoutingSession.session;
        if (updatedScoutingSession.isDisconnecting)
        {
            scoutingSessions.Remove(session.Id);
        }
        else
        {
            scoutingSessions.Add(session.Id, session);
        }
    }

    private async Task ReceivedMessage((Guid sender, Message message) receivedMessage)
    {
        Message message = receivedMessage.message;
        if (messageHandlers.TryGetValue(message.Type, out var messageHandler))
        {
            if (messageHandler is not null)
            {
                await messageHandler.Execute(receivedMessage.sender, message);
            }
        }
        else
        {
            Console.WriteLine($"Unknown message: type: {message.Type} with message: {message.Json}");
        }
    }

    protected override TcpSession CreateSession() => new ScoutingSession(this);
    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"Scouting TCP server caught an error with code {error}");
    }

    public async Task Update()
    {
        if (!isInit)
        {
            isInit = true;
            await Init();
        }

        await UpdatedScoutingSessions.Poll();
        await ReceivedMessageQueue.Poll();
    }

    private async Task Init()
    {
        //var eventTeamsJsonDocs = await mongo.Values.FindAsync(Builders<BsonDocument>.Filter.Exists("event_teams_json"));
        //eventTeamsJson.
        //tba.LoadEventMatches();
        Tba.UpdateEventKey("2022new");

        // TODO delete event_matches_json whenever it becomes invalid
        // TODO delete event_teams_json whenever it becomes invalid

        //await mongo.Values.DeleteManyAsync(Builders<BsonDocument>.Filter.Exists("event_matches_json"));

        bool loadedEventMatches = await Values.TryLoadExisting("event_matches_json", json => Tba.LoadEventMatches(json));
        if (!loadedEventMatches)
        {
            Console.WriteLine("downloading event matches json");
            await Tba.UpdateEventMatches();

            await Mongo.Values.UpdateOneAsync(
                Builders<BsonDocument>.Filter
                .Exists("event_matches_json"),
                Builders<BsonDocument>.Update
                .Set("event_matches_json", Tba.EventMatchesJson),
                new UpdateOptions() { IsUpsert = true }
            );
            Console.WriteLine("downloaded and loaded event matches json");
        }

        bool loadedEventTeams = await Values.TryLoadExisting("event_teams_json", json => Tba.LoadEventTeams(json));

        if (Tba.EventTeams is null)
        {
            Console.WriteLine("downloading event teams json");
            await Tba.UpdateEventTeams();

            await Mongo.Values.UpdateOneAsync(
                Builders<BsonDocument>.Filter
                .Exists("event_teams_json"),
                Builders<BsonDocument>.Update
                .Set("event_teams_json", Tba.EventTeamsJson),
                new UpdateOptions() { IsUpsert = true }
            );
            Console.WriteLine("downloaded and loaded event teams json");
        }

        await Mongo.Values.UpdateOneAsync(
            Builders<BsonDocument>.Filter
            .Exists("event_key"),
            Builders<BsonDocument>.Update
            .Set("event_key", "2022new"),
            new UpdateOptions() { IsUpsert = true }
        );
    }

    #region Message Handling
    private void AddMessageHandler<T>(Action<Guid, T> messageHandler)
    {
        AddMessageHandler<T>(new MessageHandler((Guid sender, Message message) =>
        {
            T? deserializedMessage = JsonSerializer.Deserialize<T>(message.Json);
            if (deserializedMessage is not null)
            {
                messageHandler(sender, deserializedMessage);
            }
        }));
    }
    private void AddMessageHandler<T>(Func<Guid, T, Task> messageHandler)
    {
        AddMessageHandler<T>(new MessageHandler(async (Guid sender, Message message) =>
        {
            T? deserializedMessage = JsonSerializer.Deserialize<T>(message.Json);
            if (deserializedMessage is not null)
            {
                await messageHandler(sender, deserializedMessage);
            }
        }));
    }
    private void AddMessageHandler<T>(MessageHandler messageHandler)
    {
        messageHandlers.Add(typeof(T).Name, messageHandler);
    }

    private void AddMessageHandlers()
    {
        AddMessageHandler((Guid sender, PingSBMessage message) =>
        {
            Console.WriteLine($"{sender}: {message.Text}");
        });

        AddMessageHandler((Guid sender, RequestValueSBMessage message) =>
        {
            if (!scoutingSessions.TryGetValue(sender, out var session))
            {
                return;
            }


            //dbClient.get

            /*
            session.SendMessage(new ProvideValueCBMessage
            {
                Name =
            });
            */
        });

        AddMessageHandler((Guid sender, UpdateValueSBMessage message) =>
        {

        });
    }
    #endregion Message Handling
}