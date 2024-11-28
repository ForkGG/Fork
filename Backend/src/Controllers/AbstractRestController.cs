using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Fork.Controllers;

[ApiController]
[Route("v1/[controller]")]
public abstract class AbstractRestController : ControllerBase
{
    protected readonly ILogger Logger;

    public AbstractRestController(ILogger logger)
    {
        Logger = logger;
    }

    protected void LogRequest()
    {
        Logger.LogDebug($"New request: {Request.Method} {Request.Path}");
    }
}