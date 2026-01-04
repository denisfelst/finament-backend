namespace Finament.Application.DTOs.Users.Requests;

public class CreateUserDto
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    // public string Password { get; set; } = "";
}