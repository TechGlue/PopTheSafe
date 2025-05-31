using Microsoft.AspNetCore.Mvc;
using MySafe.AdminCodeGenerator;
using MySafe.SafeHelper;

namespace MySafe.Controllers;

public class SafeController : BaseController
{
    private readonly ILogger<SafeController> _logger;
    private readonly SafeCache _safeCache;

    public SafeController(ILogger<SafeController> logger, SafeCache safeCache)
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
        ISafe currentSafe = _safeCache.FetchSafe(id);
        return Ok(SafeResponse.Ok(currentSafe.Describe(), currentSafe.DescribeId()));
    }

    [HttpPut("{safeId}/{safePin}")]
    [ProducesResponseType(typeof(SafeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public IActionResult Submit(int safeId, string safePin)
    {
        ISafe safe = _safeCache.FetchSafe(safeId);

        safe.SetCode(safePin, result => _ = result);

        return Ok(SafeResponse.Ok(safe.Describe(), safe.DescribeId()));
    }

    [HttpGet("reset/{safeId}")]
    [ProducesResponseType(typeof(SafeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public IActionResult Reset(int safeId)
    {
        ISafe safe = _safeCache.FetchSafe(safeId);

        safe.PressReset();

        return Ok(SafeResponse.Ok(safe.Describe(), safe.DescribeId()));
    }

    [HttpGet("open/{safeId}")]
    [ProducesResponseType(typeof(SafeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public IActionResult Open(int safeId)
    {
        ISafe safe = _safeCache.FetchSafe(safeId);

        safe.Open();

        return Ok(SafeResponse.Ok(safe.Describe(), safe.DescribeId()));
    }

    [HttpGet("close/{safeId}")]
    [ProducesResponseType(typeof(SafeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public IActionResult Close(int safeId)
    {
        ISafe safe = _safeCache.FetchSafe(safeId);

        safe.Close();

        return Ok(SafeResponse.Ok(safe.Describe(), safe.DescribeId()));
    }

    [HttpGet("lock/{safeId}")]
    [ProducesResponseType(typeof(SafeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public IActionResult Lock(int safeId)
    {
        ISafe safe = _safeCache.FetchSafe(safeId);

        safe.PressLock();

        return Ok(SafeResponse.Ok(safe.Describe(), safe.DescribeId()));
    }
    
    [HttpGet("factoryreset/{safeId}")]
    [ProducesResponseType(typeof(SafeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public IActionResult FactoryReset(int safeId)
    {
        ISafe safe = _safeCache.FactoryResetSafe(safeId);

        return Ok(SafeResponse.Ok(safe.Describe(), safe.DescribeId()));
    }
}