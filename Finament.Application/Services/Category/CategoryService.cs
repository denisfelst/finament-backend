using System.Text.RegularExpressions;
using Finament.Application.DTOs.Categories;
using Finament.Application.DTOs.Categories.Requests;
using Finament.Application.Exceptions;
using Finament.Application.Infrastructure;
using Finament.Application.Mapping;
using Microsoft.EntityFrameworkCore;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace Finament.Application.Services.Category;

public sealed class CategoryService : ICategoryService
{
    private readonly IFinamentDbContext _db;

    private static readonly Regex HexColorRegex =
        new(@"^#[0-9A-Fa-f]{6}$", RegexOptions.Compiled);

    public CategoryService(IFinamentDbContext db)
    {
        _db = db;
    }
    
    public async Task<IReadOnlyList<CategoryResponseDto>> GetByUserAsync(int userId)
    {
        var categories = await _db.Categories
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.Name)
            .ToListAsync();

        var categoryIds = categories.Select(c => c.Id).ToList();

        var expenseStats = await _db.Expenses
            .Where(e => categoryIds.Contains(e.CategoryId))
            .GroupBy(e => e.CategoryId)
            .Select(g => new { CategoryId = g.Key, Count = g.Count(), TotalSpent = (int)g.Sum(e => e.Amount) })
            .ToDictionaryAsync(x => x.CategoryId, x => new { x.Count, x.TotalSpent });

        return categories
            .Select(c =>
            {
                var stats = expenseStats.GetValueOrDefault(c.Id);
                return CategoryMapping.ToDto(c, stats?.Count ?? 0, stats?.TotalSpent ?? 0);
            })
            .ToList();
    }
    
    public async Task<CategoryResponseDto> CreateAsync(int userId, CreateCategoryDto dto)
    {
        NormalizeAndValidate(dto);

        var duplicate = await _db.Categories
            .AnyAsync(c => c.UserId == userId && c.Name == dto.Name);

        if (duplicate)
            throw new BusinessRuleException(
                "Category name already exists for this user."
            );

        var category = CategoryMapping.ToEntity(dto, userId);

        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        return CategoryMapping.ToDto(category);
    }
    
    public async Task<CategoryResponseDto> UpdateAsync(int userId, int id, UpdateCategoryDto dto)
    {
        var category = await _db.Categories.FindAsync(id);
        if (category == null)
            throw new NotFoundException("Category not found.");

        NormalizeAndValidate(dto);

        var duplicate = await _db.Categories.AnyAsync(c =>
            c.UserId == category.UserId &&
            c.Name == dto.Name &&
            c.Id != id
        );

        if (duplicate)
            throw new BusinessRuleException(
                "Category name already exists for this user."
            );

        CategoryMapping.UpdateEntity(category, dto);
        await _db.SaveChangesAsync();

        return CategoryMapping.ToDto(category);
    }
    
    public async Task DeleteAsync(int id)
    {
        var category = await _db.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        if (category == null)
            throw new NotFoundException("Category not found.");

        // Delete expenses first with separate save
        var expenses = await _db.Expenses
            .Where(e => e.CategoryId == id)
            .ToListAsync();

        if (expenses.Count > 0)
        {
            _db.Expenses.RemoveRange(expenses);
            await _db.SaveChangesAsync();
        }

        // Re-fetch and delete category
        var categoryToDelete = await _db.Categories.FindAsync(id);
        if (categoryToDelete != null)
        {
            _db.Categories.Remove(categoryToDelete);
            await _db.SaveChangesAsync();
        }
    }
    
    private void NormalizeAndValidate(ICategoryWriteBaseDto dto)
    {
        // NAME
        if (string.IsNullOrWhiteSpace(dto.Name) || dto.Name.Trim().Length < 3)
            throw new ValidationException(
                "Category name must contain at least 3 characters."
            );

        dto.Name = dto.Name.Trim();

        // MONTHLY LIMIT
        var rounded = (int)Math.Round(
            dto.MonthlyLimit,
            0,
            MidpointRounding.AwayFromZero
        );

        if (rounded < 1)
            throw new ValidationException(
                "Monthly estimate must be at least 1."
            );

        dto.MonthlyLimit = rounded;

        // COLOR
        if (string.IsNullOrWhiteSpace(dto.Color) ||
            !HexColorRegex.IsMatch(dto.Color))
        {
            dto.Color = "#FFFFFF";
        }
    }
}