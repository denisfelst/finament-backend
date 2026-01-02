namespace Finament.Application.DTOs.Expenses.Requests;

public class UpdateExpenseDto: IExpenseWriteBaseDto
{
    public required int CategoryId { get; set; }
    public required  decimal Amount { get; set; }
    public required  DateTime Date { get; set; }
    public string? Tag { get; set; }
}