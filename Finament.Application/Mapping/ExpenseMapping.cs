using Finament.Application.DTOs.Expenses;
using Finament.Application.DTOs.Expenses.Requests;
using Finament.Domain.Entities;

namespace Finament.Application.Mapping;

public static class ExpenseMapping
{
    public static ExpenseResponseDto ToDto(Expense expense)
    {
        return new ExpenseResponseDto
        {
            Id = expense.Id,
            UserId = expense.UserId,
            CategoryId = expense.CategoryId,
            Amount = expense.Amount,
            Date = expense.Date,
            Tag = expense.Tag ?? string.Empty,
            CreatedAt = expense.CreatedAt
        };
    }

    public static Expense ToEntity(CreateExpenseDto dto)
    {
        return new Expense
        {
            UserId = dto.UserId,
            CategoryId = dto.CategoryId,
            Amount = dto.Amount,
            Date = dto.Date,
            Tag = dto.Tag,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateEntity(Expense expense, UpdateExpenseDto dto)
    {
        expense.CategoryId = dto.CategoryId;

        expense.Amount = dto.Amount;

        expense.Date = dto.Date;

        if (dto.Tag != null)
            expense.Tag = dto.Tag;
    }
}