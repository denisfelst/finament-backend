namespace Finament.Application.DTOs.Expenses.Requests;

public class UpdateExpenseDto
{
    public int? CategoryId { get; set; }
    public decimal? Amount { get; set; }
    public DateTime? Date { get; set; }
    public string? Tag { get; set; }
}