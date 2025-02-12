using Microsoft.AspNetCore.Mvc;
using MySafe.AdminCodeGenerator;
using MySafe.SafeHelper;

namespace MySafe.Controllers;

/*
 * Todo: Have a request go out. If the id is present nice then fetch it and use it. Else create a new one and inject it into the dictionary.
 *
 * Todo: First figure out the fetching with id based on safes in the dummy data. Then work on the safe creation.
 *
 * Note: controllers are instantiated at request so the contructor will not live out at the same state of the current controller. Each call we reinstanciate the object again
 */

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
        
        return Ok();
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