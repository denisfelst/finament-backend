using Finament.Application.DTOs.Auth;
using Finament.Application.DTOs.Auth.Requests;
using Finament.Application.Exceptions;
using Finament.Application.Infrastructure;
using Finament.Application.Security;
using Microsoft.EntityFrameworkCore;

namespace Finament.Application.Services.Auth;

public sealed class AuthService : IAuthService
{
    private readonly IFinamentDbContext _db;
    private readonly ITokenService _tokenService;

    public AuthService(IFinamentDbContext db, ITokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) ||
            string.IsNullOrWhiteSpace(dto.Password))
        {
            throw new ValidationException("Invalid credentials.");
        }

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        
        if (user == null || !PasswordHasher.Verify(dto.Password, user.PasswordHash))
            throw new ValidationException("Invalid credentials.");

        var token = _tokenService.CreateToken(user.Id);

        return new AuthResponseDto
        {
            Token = token
        };
    }
}