using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ErrosController : ControllerBase
{
    [HttpGet("divisao-zero")]
    public ActionResult DivisaoZero()
    {
        var numero = 1;
        var zero = 0;
        _ = numero / zero;

        return Ok();
    }
}
