using Finament.Api.Persistence;
using Finament.Domain.Entities;
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

        if (settings == null)
            return Ok(null); // no related setting

        return Ok(settings);
    }
    
    [HttpPut]
    public async Task<IActionResult> Upsert([FromBody] Setting input)
    {
        if (string.IsNullOrWhiteSpace(input.Currency))
            return BadRequest(new { message = "Currency is required." });

        if (input.CycleStartDay < 1 || input.CycleStartDay > 31)
            return BadRequest(new { message = "CycleStartDay must be between 1 and 31." });

        var existing = await _db.Settings
            .FirstOrDefaultAsync(s => s.UserId == input.UserId);

        if (existing == null)
        {
            // create
            _db.Settings.Add(input);
        }
        else
        {
            // update
            existing.Currency = input.Currency;
            existing.CycleStartDay = input.CycleStartDay;
        }

        await _db.SaveChangesAsync();
        return Ok(input);
    }
}