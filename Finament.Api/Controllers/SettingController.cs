using Finament.Application.DTOs.Settings.Requests;
using Finament.Application.Services.Settings;
using Microsoft.AspNetCore.Mvc;

namespace Finament.Api.Controllers;

[ApiController]
[Route("api/settings")]
public class SettingController : ControllerBase
{
    private readonly ISettingService _service;

    public SettingController(ISettingService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int userId)
    {
        var result = await _service.GetByUserAsync(userId);
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> Upsert([FromBody] UpsertSettingDto dto)
    {
        var result = await _service.UpsertAsync(dto);
        return Ok(result);
    }
}