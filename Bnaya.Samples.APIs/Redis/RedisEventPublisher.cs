using StackExchange.Redis;

namespace Bnaya.Samples.Common;

public class RedisEventPublisher : IEventPublisher
{
    private readonly IConnectionMultiplexer _redisConnection;

    public RedisEventPublisher(IConnectionMultiplexer redisConnection)
    {
        _redisConnection = redisConnection ?? throw new ArgumentNullException(nameof(redisConnection));
    }

    public async Task<long> PublishEventAsync(MyEvent myEvent)
    {
        var redis = _redisConnection.GetDatabase();
        var message = $"{myEvent.Message} at {DateTime.Now}";

        // Publish the message to the Redis Pub/Sub channel
        var result = await redis.PublishAsync(Constants.PUB_SUB_URI, message);
        return result;
    }
}
