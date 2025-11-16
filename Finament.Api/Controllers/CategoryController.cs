using Finament.Api.Persistence;
using Finament.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Finament.Api.Controllers;

[ApiController]
[Route("api/categories")]

public class CategoryController : ControllerBase
{
    private readonly FinamentDbContext _db;

    public CategoryController(FinamentDbContext db)
    {
        _db = db;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int userId)
    {
        var categories = await _db.Categories
            .Where(c => c.UserId == userId)
            .ToListAsync();

        return Ok(categories);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(Category category)
    {
        if (string.IsNullOrWhiteSpace(category.Name))
            return BadRequest(new { message = "Name is required." });
        
        var duplicate = await _db.Categories
            .AnyAsync(c => c.UserId == category.UserId && c.Name == category.Name);
        if (duplicate)
            return Conflict(new { message = "Category name already exists for this user." });
        
        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        return Ok(category);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Category updatedCategory)
    {
        var category = await _db.Categories.FindAsync(id);
        if (category == null)
            return NotFound(new { message = "Category not found." });
        
        if (updatedCategory.UserId != category.UserId)
            return BadRequest(new { message = "UserId mismatch." });
        
        if (string.IsNullOrWhiteSpace(updatedCategory.Name))
            return BadRequest(new { message = "Name is required." });
        
        bool duplicate = await _db.Categories
            .AnyAsync(c =>
                c.UserId == category.UserId &&
                c.Name == updatedCategory.Name &&
                c.Id != id);

        if (duplicate)
            return Conflict(new { message = "Category name already exists for this user." });
        
        category.Name = updatedCategory.Name;
        category.MonthlyLimit = updatedCategory.MonthlyLimit;
        category.Color = updatedCategory.Color;

        await _db.SaveChangesAsync();

        return Ok(category);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _db.Categories.FindAsync(id);

        if (category == null)
            return NotFound(new { message = "Category not found." });
        
        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();
        
        return NoContent();
    }
}