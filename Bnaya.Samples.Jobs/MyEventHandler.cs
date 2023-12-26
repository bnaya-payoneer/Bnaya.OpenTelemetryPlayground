using Bnaya.Samples.Common;

internal class MyEventHandler : IEventHandler
{
    private readonly ILogger<MyEventHandler> _logger;

    public MyEventHandler(ILogger<MyEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleEventAsync(MyEvent myEvent)
    {
        _logger.LogInformation("Event: {message}", myEvent.Message);
        return Task.CompletedTask;
    }
}