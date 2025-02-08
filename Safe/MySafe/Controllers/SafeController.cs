using Microsoft.AspNetCore.Mvc;
using MySafe.SafeHelper;

namespace MySafe.Controllers;

public class SafeController : BaseController
{
    private readonly ILogger<SafeController> _logger;
    private readonly ISafe _mySafe;

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
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public IActionResult Submit(string safePin)
    {
        _logger.LogWarning("This is the giving pin {safePin}", safePin);
        SafeResponse safeResponse = new SafeResponse();

        if (safePin.Length == 0)
        {

            return BadRequest(new SafeResponse
            {
                IsDetail = "safe pin is empty or null. check safe input", 
                IsSuccessful = false
            });
        }
        
        _mySafe.SetCode(safePin, result => safeResponse = result);
        
        return Ok(SafeResponse.Ok(_mySafe.Describe()));
    }
    
    [HttpGet("reset")]
    [ProducesResponseType(typeof(SafeResponse), StatusCodes.Status200OK)]
    public IActionResult Reset()
    {
        _mySafe.PressReset();
        return Ok(SafeResponse.Ok(_mySafe.Describe()));
    }
    
    [HttpGet("open")]
    [ProducesResponseType(typeof(SafeResponse), StatusCodes.Status200OK)]
    public IActionResult Open()
    {
        _mySafe.Open();
        return Ok(SafeResponse.Ok(_mySafe.Describe()));
    }
    
    [HttpGet("close")]
    [ProducesResponseType(typeof(SafeResponse), StatusCodes.Status200OK)]
    public IActionResult Close()
    {
        _mySafe.Close();
        return Ok(SafeResponse.Ok(_mySafe.Describe()));
    }
    
    [HttpGet("lock")]
    [ProducesResponseType(typeof(SafeResponse), StatusCodes.Status200OK)]
    public IActionResult Lock()
    {
        _mySafe.PressLock();
        return Ok(SafeResponse.Ok(_mySafe.Describe()));
    }
}