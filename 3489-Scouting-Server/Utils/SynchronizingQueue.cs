namespace ScoutingServer3489.Utils;

public class SynchronizingQueue<T>
{
    private readonly Action<T>? syncHandler = null;
    private readonly Func<T, Task>? asyncHandler = null;

    private readonly ConcurrentQueue<T> queue = new();

    public SynchronizingQueue(Action<T> syncHandler)
    {
        this.syncHandler = syncHandler;
    }

    public SynchronizingQueue(Func<T, Task> asyncHandler)
    {
        this.asyncHandler = asyncHandler;
    }

    public void Enqueue(T value)
    {
        queue.Enqueue(value);
    }

    public async Task Poll()
    {
        int count = queue.Count;
        for (int i = 0; i < count; i++)
        {
            if (queue.TryDequeue(out var value))
            {
                if (syncHandler is not null)
                {
                    syncHandler(value);
                }
                if (asyncHandler is not null)
                {
                    await asyncHandler(value);
                }
            }
            else
            {
                break;
            }
        }
    }
}