using System.Text.RegularExpressions;
using Finament.Api.Persistence;
using Finament.Application.DTOs.Categories.Requests;
using Finament.Application.Mapping;
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
            .OrderBy(c => c.Name)
            .ToListAsync();

        var dtos = categories.Select(CategoryMapping.ToDto).ToList();
        return Ok(dtos);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CreateCategoryDto dto)
    {
        var validation = ValidateAndNormalizeCategory(dto);
        if (validation != null)
            return validation;

        var duplicate = await _db.Categories
            .AnyAsync(c => c.UserId == dto.UserId && c.Name == dto.Name);

        if (duplicate)
            return Conflict(new
            {
                message = "Category name already exists for this user."
            });

        var category = CategoryMapping.ToEntity(dto);

        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        return Ok(CategoryMapping.ToDto(category));
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateCategoryDto dto)
    {
        var category = await _db.Categories.FindAsync(id);
        if (category == null)
            return NotFound(new { message = "Category not found." });

        var validation = ValidateAndNormalizeCategory(dto);
        if (validation != null)
            return validation;

        var duplicate = await _db.Categories
            .AnyAsync(c =>
                c.UserId == category.UserId &&
                c.Name == dto.Name &&
                c.Id != id
            );

        if (duplicate)
            return Conflict(new
            {
                message = "Category name already exists for this user."
            });

        CategoryMapping.UpdateEntity(category, dto);

        await _db.SaveChangesAsync();

        return Ok(CategoryMapping.ToDto(category));
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
    
    private static readonly Regex HexColorRegex =
        new(@"^#[0-9A-Fa-f]{6}$", RegexOptions.Compiled);

    private IActionResult? ValidateAndNormalizeCategory(ICategoryWriteBaseDto dto)
    {
        // === NAME ===
        if (string.IsNullOrWhiteSpace(dto.Name) || dto.Name.Trim().Length < 3)
        {
            return BadRequest(new
            {
                message = "Category name must contain at least 3 characters."
            });
        }

        dto.Name = dto.Name.Trim();

        // === MONTHLY ESTIMATE ===
        var roundedEstimate = (int)Math.Round(
            dto.MonthlyLimit,
            0,
            MidpointRounding.AwayFromZero
        );

        if (roundedEstimate < 1)
        {
            return BadRequest(new
            {
                message = "Monthly estimate must be at least 1."
            });
        }

        dto.MonthlyLimit = roundedEstimate;

        // === COLOR ===
        if (string.IsNullOrWhiteSpace(dto.Color) || !HexColorRegex.IsMatch(dto.Color))
        {
            dto.Color = "#FFFFFF";
        }

        return null;
    }
}