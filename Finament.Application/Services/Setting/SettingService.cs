using Finament.Application.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Finament.Application.DTOs.Settings.Requests;
using Finament.Application.DTOs.Settings;
using Finament.Application.Exceptions;
using Finament.Application.Mapping;

namespace Finament.Application.Services.Setting;

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

    public async Task<SettingResponseDto> UpsertAsync(int userId, UpsertSettingDto dto)
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
            .AnyAsync(u => u.Id == userId);

        if (!userExists)
            throw new NotFoundException("User does not exist.");

        // UPSERT
        var settings = await _db.Settings
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (settings == null)
        {
            settings = SettingMapping.Create(userId, dto);
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
