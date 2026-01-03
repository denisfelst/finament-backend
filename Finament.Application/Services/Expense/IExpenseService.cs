
using Finament.Application.DTOs.Expenses.Requests;
using Finament.Application.DTOs.Expenses;

namespace Finament.Application.Services.Expense;

public interface IExpenseService
{
    Task<IReadOnlyList<ExpenseResponseDto>> GetByUserAsync(int userId);
    Task<ExpenseResponseDto> CreateAsync(CreateExpenseDto dto);
    Task<ExpenseResponseDto> UpdateAsync(int id, UpdateExpenseDto dto);
    Task DeleteAsync(int id);
}