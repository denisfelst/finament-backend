using Finament.Application.DTOs.Categories;
using Finament.Application.DTOs.Categories.Requests;
using Finament.Domain.Entities;

namespace Finament.Application.Mapping;

public static class CategoryMapping
{
    public static CategoryResponseDto ToDto(Category category, int expenseCount = 0)
    {
        return new CategoryResponseDto
        {
            Id = category.Id,
            UserId = category.UserId,
            Name = category.Name,
            MonthlyLimit = category.MonthlyLimit,
            Color = category.Color,
            CreatedAt = category.CreatedAt,
            ExpenseCount = expenseCount
        };
    }

    public static Category ToEntity(CreateCategoryDto dto, int userId)
    {
        return new Category
        {
            UserId = userId,
            Name = dto.Name,
            MonthlyLimit = dto.MonthlyLimit,
            Color = dto.Color,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateEntity(Category category, UpdateCategoryDto dto)
    {

        category.Name = dto.Name;

        category.MonthlyLimit = dto.MonthlyLimit;

        category.Color = dto.Color;
    }
}