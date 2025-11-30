namespace Finament.Application.DTOs.Expenses.Requests;

public class CreateExpenseDto
{
    public int UserId { get; set; }
    public int CategoryId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Tag { get; set; } = "";
}