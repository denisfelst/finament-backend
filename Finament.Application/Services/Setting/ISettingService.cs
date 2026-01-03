using Finament.Application.DTOs.Settings.Requests;
using Finament.Application.DTOs.Settings;

namespace Finament.Application.Services.Settings;

public interface ISettingService
{
    Task<SettingResponseDto?> GetByUserAsync(int userId);
    Task<SettingResponseDto> UpsertAsync(UpsertSettingDto dto);
}
