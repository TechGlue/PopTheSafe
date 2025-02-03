using Microsoft.AspNetCore.Mvc;
using Safe;

namespace MySafe.Controllers;

public class SafeController : BaseController
{
    private readonly ILogger<SafeController> _logger;

    public SafeController(ILogger<SafeController> logger)
    {
        _logger = logger;

    }

    [HttpGet]
    [ProducesResponseType(typeof(SafeResponse), StatusCodes.Status200OK)]
    public IActionResult Safe()
    {
        _logger.LogInformation("Safe controller successful");
        return Ok(SafeResponse.Ok("Safe API/Controllers successful"));
    }
}