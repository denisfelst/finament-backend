using Finament.Application.DTOs.Settings.Requests;
using Finament.Application.Mapping;
using Finament.Domain.Entities;
using Finament.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Finament.Api.Controllers;

[ApiController]
[Route("api/settings")]
public class SettingController : ControllerBase
{
    private readonly FinamentDbContext _db;

    public SettingController(FinamentDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int userId)
    {
        var settings = await _db.Settings
            .FirstOrDefaultAsync(s => s.UserId == userId);

        return Ok(settings == null ? null : // no related setting
            SettingMapping.ToDto(settings));
    }
    
    [HttpPut]
    public async Task<IActionResult> Upsert([FromBody] UpsertSettingDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Currency))
            return BadRequest(new { message = "Currency is required." });

        if (dto.CycleStartDay is < 1 or > 31)
            return BadRequest(new { message = "CycleStartDay must be between 1 and 31." });

        var userExists = await _db.Users.AnyAsync(u => u.Id == dto.UserId);
        if (!userExists)
            return NotFound(new { message = "User does not exist." });

        var settings = await _db.Settings
            .FirstOrDefaultAsync(s => s.UserId == dto.UserId);

        if (settings == null)
        {
            settings = SettingMapping.Create(dto);
            _db.Settings.Add(settings);
        }
        else
        {
            SettingMapping.UpdateEntity(settings, dto);
        }

        await _db.SaveChangesAsync();

        return Ok(SettingMapping.ToDto(settings));
    }
}