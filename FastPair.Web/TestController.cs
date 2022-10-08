using Microsoft.AspNetCore.Mvc;

namespace FastPair.Web;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("token")]
    public async Task<IActionResult> GetTokenAsync()
    {
        return Ok("ewfewf");
    }
}