using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using MySafe.AdminCodeGenerator;
using MySafe.SafeHelper;

namespace MySafe.Controllers;

/* Add following end points
 * Have an intro where you ask for a safe id, you enter or if safe id comes back invalid then you 
 * Create a safe
 * if it's not there 
 * 
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
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public IActionResult Safe()
    {
        return Ok(SafeResponse.Ok("Safe API/Controllers successful"));
    }

    [HttpGet("status/{id}")]
    [ProducesResponseType(typeof(SafeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public IActionResult FetchSafeStatus(int id)
    {
        return Ok(SafeResponse.Ok(_safeCache.FetchSafe(id).Describe()));
    }

    [HttpPut("{safeId}/{safePin}")]
    [ProducesResponseType(typeof(SafeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public IActionResult Submit(int safeId, string safePin)
    {
        ISafe safe = _safeCache.FetchSafe(safeId);

        safe.SetCode(safePin, result => _ = result);

        return Ok(SafeResponse.Ok(safe.Describe()));
    }

    [HttpGet("reset/{safeId}")]
    [ProducesResponseType(typeof(SafeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public IActionResult Reset(int safeId)
    {
        ISafe safe = _safeCache.FetchSafe(safeId);

        safe.PressReset();

        return Ok(SafeResponse.Ok(safe.Describe()));
    }

    [HttpGet("open/{safeId}")]
    [ProducesResponseType(typeof(SafeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public IActionResult Open(int safeId)
    {
        ISafe safe = _safeCache.FetchSafe(safeId);

        safe.Open();

        return Ok(SafeResponse.Ok(safe.Describe()));
    }

    [HttpGet("close/{safeId}")]
    [ProducesResponseType(typeof(SafeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public IActionResult Close(int safeId)
    {
        ISafe safe = _safeCache.FetchSafe(safeId);

        safe.Close();

        return Ok(SafeResponse.Ok(safe.Describe()));
    }

    [HttpGet("lock/{safeId}")]
    [ProducesResponseType(typeof(SafeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public IActionResult Lock(int safeId)
    {
        ISafe safe = _safeCache.FetchSafe(safeId);

        safe.PressLock();

        return Ok(SafeResponse.Ok(safe.Describe()));
    }
}