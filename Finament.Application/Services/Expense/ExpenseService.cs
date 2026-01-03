using Finament.Application.DTOs.Expenses;
using Finament.Application.DTOs.Expenses.Requests;
using Finament.Application.Exceptions;
using Finament.Application.Infrastructure;
using Finament.Application.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Finament.Application.Services.Expense;

public sealed class ExpenseService : IExpenseService
{
    private readonly IFinamentDbContext _db;

    public ExpenseService(IFinamentDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<ExpenseResponseDto>> GetByUserAsync(int userId)
    {
        var expenses = await _db.Expenses
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.Date)
            .ToListAsync();

        return expenses.Select(ExpenseMapping.ToDto).ToList();
    }

    public async Task<ExpenseResponseDto> CreateAsync(CreateExpenseDto dto)
    {
        await NormalizeAndValidateAsync(dto.UserId, dto);

        var expense = ExpenseMapping.ToEntity(dto);

        _db.Expenses.Add(expense);
        await _db.SaveChangesAsync();

        return ExpenseMapping.ToDto(expense);
    }

    public async Task<ExpenseResponseDto> UpdateAsync(int id, UpdateExpenseDto dto)
    {
        var expense = await _db.Expenses.FindAsync(id);
        if (expense == null)
            throw new NotFoundException("Expense not found.");

        await NormalizeAndValidateAsync(expense.UserId, dto);

        ExpenseMapping.UpdateEntity(expense, dto);
        await _db.SaveChangesAsync();

        return ExpenseMapping.ToDto(expense);
    }

    public async Task DeleteAsync(int id)
    {
        var expense = await _db.Expenses.FindAsync(id);
        if (expense == null)
            throw new NotFoundException("Expense not found.");

        _db.Expenses.Remove(expense);
        await _db.SaveChangesAsync();
    }


    private async Task NormalizeAndValidateAsync(
        int userId,
        IExpenseWriteBaseDto dto
    )
    {
        // AMOUNT
        var rounded = (int)Math.Round(
            dto.Amount,
            0,
            MidpointRounding.AwayFromZero
        );

        if (rounded < 1)
            throw new ValidationException("Amount must be at least 1.");

        dto.Amount = rounded;

        // DATE
        if (dto.Date.Date > DateTime.UtcNow.Date)
            throw new ValidationException(
                "Expense date must be today or in the past."
            );

        // CATEGORY
        var categoryExists = await _db.Categories.AnyAsync(c =>
            c.Id == dto.CategoryId && c.UserId == userId
        );

        if (!categoryExists)
            throw new ValidationException("Invalid category.");

        // TAG
        dto.Tag = string.IsNullOrWhiteSpace(dto.Tag)
            ? null
            : ToCamelCase(dto.Tag);
    }

    private static string ToCamelCase(string input)
    {
        var raw = input.Trim().TrimStart('#');

        var words = raw.Split(
            ' ',
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
        );

        if (words.Length == 0)
            return null!;

        var camelCase =
            words[0].ToLowerInvariant() +
            string.Concat(words.Skip(1)
                .Select(w =>
                    char.ToUpperInvariant(w[0]) +
                    w.Substring(1).ToLowerInvariant()
                ));

        return $"#{camelCase}";
    }
}