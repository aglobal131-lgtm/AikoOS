namespace AikoOS.Behavior.Bus;

internal sealed class EventSubscription : IDisposable
{
    private readonly Action _unsubscribe;
    private bool _isDisposed;

    public EventSubscription(Action unsubscribe)
    {
        _unsubscribe = unsubscribe;
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _unsubscribe();
        _isDisposed = true;
    }
}