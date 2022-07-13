using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoutingServer3489.Utils;

public class MessageHandler
{
    private readonly Action<Guid, Message>? syncMessageHandler = null;
    private readonly Func<Guid, Message, Task>? asyncMessageHandler = null;

    public MessageHandler(Action<Guid, Message>? syncMessageHandler)
    {
        this.syncMessageHandler = syncMessageHandler;
    }

    public MessageHandler(Func<Guid, Message, Task>? asyncMessageHandler)
    {
        this.asyncMessageHandler = asyncMessageHandler;
    }

    public async Task Execute(Guid sender, Message message)
    {
        if (syncMessageHandler is not null)
        {
            syncMessageHandler(sender, message);
        }
        if (asyncMessageHandler is not null)
        {
            await asyncMessageHandler(sender, message);
        }
    }
}