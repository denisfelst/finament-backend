namespace Finament.Application.DTOs.Expenses;

public class ExpenseResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CategoryId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Tag { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}