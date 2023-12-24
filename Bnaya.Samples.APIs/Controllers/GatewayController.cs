using Microsoft.AspNetCore.Mvc;

namespace Bnaya.Samples.APIs.Controllers;
[ApiController]
[Route("[controller]")]
public class GatewayController : ControllerBase
{
    private readonly ILogger<GatewayController> _logger;

    public GatewayController(ILogger<GatewayController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async Task PostAsync(string name)
    {
       await Task.Delay(1000);
    }
}
