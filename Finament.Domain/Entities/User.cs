namespace Finament.Domain.Entities;

public class User
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string Email { get; set; } = "...";
    
    public DateTime CreatedAt { get; set; }
}