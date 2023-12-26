namespace Bnaya.Samples.Common;

public interface IEventHandler
{
    Task HandleEventAsync(MyEvent myEvent);
}