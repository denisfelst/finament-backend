using Finament.Application.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Finament.Application.Services.Settings;

using Exceptions;
using Mapping;
using Finament.Application.DTOs.Settings.Requests;
using Finament.Application.DTOs.Settings;

public sealed class SettingService : ISettingService
{
    private readonly IFinamentDbContext _db;

    public SettingService(IFinamentDbContext db)
    {
        _db = db;
    }

    public async Task<SettingResponseDto?> GetByUserAsync(int userId)
    {
        var settings = await _db.Settings.FirstOrDefaultAsync(s => s.UserId == userId);

        return settings == null
            ? null
            : SettingMapping.ToDto(settings);
    }

    public async Task<SettingResponseDto> UpsertAsync(UpsertSettingDto dto)
    {
        // CURRENCY
        if (string.IsNullOrWhiteSpace(dto.Currency))
            throw new ValidationException("Currency is required.");

        // CYCLE START DAY
        if (dto.CycleStartDay is < 1 or > 31)
            throw new ValidationException(
                "CycleStartDay must be between 1 and 31."
            );

        // USER EXISTS
        var userExists = await _db.Users
            .AnyAsync(u => u.Id == dto.UserId);

        if (!userExists)
            throw new NotFoundException("User does not exist.");

        // UPSERT
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

        return SettingMapping.ToDto(settings);
    }
}
