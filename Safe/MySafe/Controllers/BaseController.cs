using Microsoft.AspNetCore.Mvc;

namespace MySafe.Controllers;

[ApiController]
[Route("v0.0.1/[controller]")]
[Produces("application/json")]
public abstract class BaseController:ControllerBase;