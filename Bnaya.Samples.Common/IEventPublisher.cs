namespace Bnaya.Samples.Common;

public interface IEventPublisher
{
    Task<long> PublishEventAsync(MyEvent myEvent);
}
