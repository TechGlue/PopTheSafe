using Microsoft.AspNetCore.Mvc;
using MySafe.SafeHelper;

namespace MySafe.Controllers;

public class SafeController : BaseController
{
    private readonly ILogger<SafeController> _logger;
    private readonly ISafe _mySafe;

    /*
     * Todo:
     * Rest calls to create
     * Submit (takes in a code parameter) - have submit take in parameters instead of a path like that. 
     * Reset (takes in no parameter it's all back end)
     * Unlock (takes in no parameter it's all back end)
     */

    public SafeController(ILogger<SafeController> logger, ISafe controllerSafe)
    {
        _logger = logger;
        _mySafe = controllerSafe;
    }

    [HttpGet]
    [ProducesResponseType(typeof(SafeResponse), StatusCodes.Status200OK)]
    public IActionResult Safe()
    {
        _logger.LogInformation("Safe controller successful");
        return Ok(SafeResponse.Ok("Safe API/Controllers successful"));
    }

    [HttpGet("status")]
    [ProducesResponseType(typeof(SafeResponse), StatusCodes.Status200OK)]
    public IActionResult FetchSafeStatus()
    {
        return Ok(SafeResponse.Ok(_mySafe.Describe()));
    }

    [HttpPut("{safePin}")]
    [ProducesResponseType(typeof(SafeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Submit(string safepin)
    {
        return Ok(safepin);
    }
    
    [HttpGet("Throw")]
    public IActionResult Throw() =>
        throw new ArgumentException("THIS IS A BIG BAD EXCEPTION");
}