using Finament.Domain.Entities;
using Finament.Application.DTOs.Users;
using Finament.Application.DTOs.Users.Requests;

namespace Finament.Application.Mapping;

public static class UserMapping
{
    public static UserResponseDto ToDto(User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };
    }

    public static User ToEntity(CreateUserDto dto, string passwordHash)
    {
        return new User
        {
            Name = dto.Name,
            Email = dto.Email,
            // PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateEntity(User user, UpdateUserDto dto)
    {
        if (dto.Name != null)
            user.Name = dto.Name;

        if (dto.Email != null)
            user.Email = dto.Email;
    }
}