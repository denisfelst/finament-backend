using Finament.Api.Persistence;
using Finament.Domain.Entities;
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

        return Ok(expenses);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(Expense expense)
    {
        // amount > 0
        if (expense.Amount <= 0)
            return BadRequest(new { message = "Amount must be greater than zero." });

        // category exists
        var category = await _db.Categories.FindAsync(expense.CategoryId);
        if (category == null)
            return BadRequest(new { message = "Category does not exist." });

        // category belongs to user
        if (category.UserId != expense.UserId)
            return BadRequest(new { message = "Category does not belong to the user." });

        // tag to camelCase
        expense.Tag = ToCamelCase(expense.Tag);

        // timestamp
        expense.CreatedAt = DateTime.UtcNow;
        
        // save
        _db.Expenses.Add(expense);
        await _db.SaveChangesAsync();

        return Ok(expense);
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

    
    private string? ToCamelCase(string? tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
            return null;

        tag = tag.Trim();

        var noSpaces = string.Concat(tag.Split(' '));
        return char.ToLower(noSpaces[0]) + noSpaces.Substring(1);
    }
    
}