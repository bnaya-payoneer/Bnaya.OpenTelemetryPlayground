
using Bnaya.Samples.Common;
using StackExchange.Redis;

namespace Bnaya.Samples.Jobs;

public class Job : IHostedService
{
    private readonly ILogger<Job> _logger;
    private readonly IEventHandler _eventHandler;
    private readonly IConnectionMultiplexer _redisConnection;
    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    private ISubscriber? _redisSubscriber;

    public Job(
        ILogger<Job> logger,
        IEventHandler eventHandler,
        IConnectionMultiplexer redisConnection)
    {
        _logger = logger;
        _eventHandler = eventHandler;
        _redisConnection = redisConnection;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        cancellationToken.Register(() => _cancellationTokenSource.Cancel());
        await SubscribeAsync(Constants.PUB_SUB_URI);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cancellationTokenSource.Cancel();
        return Task.CompletedTask;
    }

    public async Task SubscribeAsync(string channelName)
    {
        ISubscriber redisSubscriber = _redisConnection.GetSubscriber();
        _redisSubscriber = redisSubscriber;

        RedisChannel channel = new RedisChannel(channelName, RedisChannel.PatternMode.Auto);
        // Subscribe to the Redis Pub/Sub channel
        await redisSubscriber.SubscribeAsync(channel, Callback);

        // TODO: log
        Console.WriteLine($"Subscribed to channel: {channelName}");

        async void Callback(RedisChannel channel, RedisValue message)
        { 
            var myEvent = new MyEvent(message);

            await _eventHandler.HandleEventAsync(myEvent);
        }
    }
}
