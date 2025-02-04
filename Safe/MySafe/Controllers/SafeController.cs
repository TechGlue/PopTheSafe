using Microsoft.AspNetCore.Mvc;
using MySafe.SafeHelper;

namespace MySafe.Controllers;

public class SafeController : BaseController
{
    private readonly ILogger<SafeController> _logger;
    private readonly ISafe _mySafe;
    
    /*
     * Todo:
     * Create a transient object
     * Create a status route to continuously fetch result on the status of that object
     * Create the main menu behavior through rest calls
     */

    public SafeController(ILogger<SafeController> logger, ISafe controllerSafe )
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
        _logger.LogInformation("Safe controller successful");
        return Ok(SafeResponse.Ok("Safe API/Controllers successful"));
    }
}