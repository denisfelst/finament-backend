namespace Finament.Application.DTOs.Categories;

public class CategoryResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = "";
    public decimal MonthlyLimit { get; set; }
    public string Color { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public int ExpenseCount { get; set; }
}