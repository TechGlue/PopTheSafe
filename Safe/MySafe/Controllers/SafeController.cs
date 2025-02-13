using Microsoft.AspNetCore.Mvc;
using MySafe.AdminCodeGenerator;
using MySafe.SafeHelper;

namespace MySafe.Controllers;

public class SafeController : BaseController
{
    private readonly ILogger<SafeController> _logger;
    private readonly SafeCache _safeCache;

    public SafeController(IAdminCodeGenerator adminCodeGenerator, ILogger<SafeController> logger, SafeCache safeCache)
    {
        _safeCache = safeCache;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(SafeResponse), StatusCodes.Status200OK)]
    public IActionResult Safe()
    {
        return Ok(SafeResponse.Ok("Safe API/Controllers successful"));
    }

    [HttpGet("status/{id}")]
    [ProducesResponseType(typeof(SafeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(SafeResponse), StatusCodes.Status200OK)]
    public IActionResult FetchSafeStatus(int id)
    {
        if (!_safeCache.ContainsSafe(id))
        {
            throw new ArgumentException("id not found");
        }
        
        return Ok(_safeCache.FetchSafe(id).Describe());
    }

    [HttpPut("{safeId}/{safePin}")]
    [ProducesResponseType(typeof(SafeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public IActionResult Submit(int safeId, string safePin)
    {
        ISafe safe = _safeCache.FetchSafe(safeId);

        safe.SetCode(safePin, result => _ = result);
        
        return Ok(safe.Describe());
    }
    
    [HttpGet("reset/{safeId}")]
    [ProducesResponseType(typeof(SafeResponse), StatusCodes.Status200OK)]
    public IActionResult Reset(int safeId)
    {
        return Ok();
    }
    
    [HttpGet("open/{safeId}")]
    [ProducesResponseType(typeof(SafeResponse), StatusCodes.Status200OK)]
    public IActionResult Open()
    {
        return Ok();
    }
    
    [HttpGet("close/{safeId}")]
    [ProducesResponseType(typeof(SafeResponse), StatusCodes.Status200OK)]
    public IActionResult Close()
    {
        return Ok();
    }
    
    [HttpGet("lock/{safeId}")]
    [ProducesResponseType(typeof(SafeResponse), StatusCodes.Status200OK)]
    public IActionResult Lock()
    {
        return Ok();
    }
}