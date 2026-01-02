namespace Finament.Application.DTOs.Expenses.Requests;

public class CreateExpenseDto: IExpenseWriteBaseDto
{
    public required int UserId { get; set; }
    public required int CategoryId { get; set; }
    public required decimal Amount { get; set; }
    public required DateTime Date { get; set; }
    public string? Tag { get; set; } = "";
}