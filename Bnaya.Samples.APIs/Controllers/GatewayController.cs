using Bnaya.Samples.Common;
using Microsoft.AspNetCore.Mvc;

namespace Bnaya.Samples.APIs.Controllers;
[ApiController]
[Route("[controller]")]
public class GatewayController : ControllerBase
{
    private readonly ILogger<GatewayController> _logger;
    private readonly IEventPublisher _eventPublisher;

    public GatewayController(
                ILogger<GatewayController> logger,
                IEventPublisher eventPublisher)
    {
        _logger = logger;
        _eventPublisher = eventPublisher;
    }

    [HttpPost]
    public async Task PostAsync(string name)
    {
        var e = new MyEvent(name);
        long id = await _eventPublisher.PublishEventAsync(e);
        _logger.LogInformation("publish: {message}", e.Message);
    }
}
