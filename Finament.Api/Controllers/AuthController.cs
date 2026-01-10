using Finament.Application.DTOs.Auth.Requests;
using Finament.Application.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Finament.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }
    
    [HttpGet("health")]
    public async Task<IActionResult> Health()
    {
        return Ok(new { status = "healthy" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto dto)
    {
        var result = await _service.LoginAsync(dto);
        return Ok(result);
    }
}
