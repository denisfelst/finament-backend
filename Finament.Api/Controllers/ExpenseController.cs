using Finament.Application.DTOs.Expenses.Requests;
using Finament.Application.Mapping;
using Finament.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Finament.Api.Controllers;

[ApiController]
[Route("api/expenses")]
public class ExpenseController : ControllerBase
{
    private readonly FinamentDbContext _db;

    public ExpenseController(FinamentDbContext db)
    {
        _db = db;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int userId)
    {
        var expenses = await _db.Expenses
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.Date)
            .ToListAsync();
        
        var dtos = expenses.Select(ExpenseMapping.ToDto).ToList();

        return Ok(dtos);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CreateExpenseDto dto)
    {
        var validation = await ValidateAndNormalizeExpense(dto.UserId, dto);
        if (validation != null)
            return validation;
        
        var expense = ExpenseMapping.ToEntity(dto);
        
        _db.Expenses.Add(expense);
        await _db.SaveChangesAsync();
        
        return Ok(ExpenseMapping.ToDto(expense));
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateExpenseDto dto)
    {
        var expense = await _db.Expenses.FindAsync(id);
        if (expense == null)
        {
            return NotFound(new { message = "Expense not found." });
        }

        var validation = await ValidateAndNormalizeExpense(expense.UserId, dto);
        if (validation != null)
            return validation;

        ExpenseMapping.UpdateEntity(expense, dto);

        await _db.SaveChangesAsync();
        return Ok(ExpenseMapping.ToDto(expense));
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var expense = await _db.Expenses.FindAsync(id);
        if (expense == null)
            return NotFound(new { message = "Expense not found." });
        
        _db.Expenses.Remove(expense);
        await _db.SaveChangesAsync();
        
        return NoContent();
    }

    private async Task<IActionResult?> ValidateAndNormalizeExpense(
        int userId,
        IExpenseWriteBaseDto dto
    )
    {
        // === AMOUNT ===
        var roundedAmount = (int)Math.Round(
            dto.Amount,
            0,
            MidpointRounding.AwayFromZero
        );

        if (roundedAmount < 1)
            return BadRequest(new { message = "Amount must be at least 1." });

        dto.Amount = roundedAmount;

        // === DATE ===
        if (dto.Date.Date >= DateTime.UtcNow.Date.AddDays(1))
            return BadRequest("Expense date must be today or in the past.");

        // === CATEGORY ===
        var category = await _db.Categories
            .FirstOrDefaultAsync(c => c.Id == dto.CategoryId && c.UserId == userId);


        if (category == null)
            return BadRequest(new { message = "Invalid category." });

        // === TAG ===
        dto.Tag = string.IsNullOrWhiteSpace(dto.Tag)
            ? null
            : ToCamelCase(dto.Tag);

        return null;
    }

    private static string ToCamelCase(string input)
    {
        // Remove leading #
        var raw = input.Trim().TrimStart('#');

        // Split by whitespace
        var words = raw
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (words.Length == 0)
        {
            return null!;
        }

        // camelCase
        var camelCase = words[0].ToLowerInvariant() + string.Concat(words.Skip(1).Select(w => char.ToUpperInvariant(w[0]) + w.Substring(1).ToLowerInvariant()));

        return $"#{camelCase}";
    }
}