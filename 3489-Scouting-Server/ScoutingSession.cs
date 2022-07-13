using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoutingServer3489;

public class ScoutingSession : TcpSession
{
    private readonly ScoutingServer server;

    public ScoutingSession(ScoutingServer server) : base(server)
    {
        this.server = server;
    }

    protected override void OnConnected()
    {
        server.UpdatedScoutingSessions.Enqueue((isDisconnecting: false, this));

        Console.WriteLine($"{Id}: connected");

        SendMessage(new PingCBMessage
        {
            Text = "Hello, World!"
        });
    }

    protected override void OnDisconnected()
    {
        server.UpdatedScoutingSessions.Enqueue((isDisconnecting: true, this));

        Console.WriteLine($"{Id}: disconnected");
    }

    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"{Id}: error: {error}");
    }

    protected override void OnReceived(byte[] buffer, long offset, long size)
    {
        string? debugMessageJson = null;
        try
        {
            string messageJson = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
            debugMessageJson = messageJson;
            string[] messageJsons = messageJson.Replace("}{", "}|{").Split('|');
            foreach (string json in messageJsons)
            {
                Message? message = JsonSerializer.Deserialize<Message>(json);
                if (message is null)
                {
                    throw new Exception($"message null");
                }
                else if (message.Version != "0")
                {
                    throw new Exception($"invalid version: {message.Version}");
                }
                else
                {
                    server.ReceivedMessageQueue.Enqueue((Id, message));
                }
            }
        }
        catch (Exception e)
        {
            Disconnect();
            string debugInfo =
                $"\n{Id}: sent invalid data, disconnected them\n" +
                $"{Id}: debug message json: {debugMessageJson}\n" +
                $"{Id}: {e}\n";
            Console.WriteLine(debugInfo);
        }
    }

    public void SendMessage<T>(T message)
    {
        string data = JsonSerializer.Serialize(new Message
        {
            Version = "0",
            Type = typeof(T).Name,
            Json = JsonSerializer.Serialize(message)
        });

        SendAsync(data);
    }
}