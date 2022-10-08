using FastPair.Admin.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace FastPair.Admin.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class UserCodesController : ControllerBase
{
    public UserCodesController(UserCodesService userCodesService)
    {
        _userCodesService = userCodesService;
    }

    private readonly UserCodesService _userCodesService;

    [HttpGet]
    public async Task SendCode(string code)
    {
        await _userCodesService.SendCode(code);
    }
}