using Finament.Application.DTOs.Categories;
using Finament.Application.DTOs.Categories.Requests;
using Finament.Domain.Entities;

namespace Finament.Application.Mapping;

public static class CategoryMapping
{
    public static CategoryResponseDto ToDto(Category category)
    {
        return new CategoryResponseDto
        {
            Id = category.Id,
            UserId = category.UserId,
            Name = category.Name,
            MonthlyLimit = category.MonthlyLimit,
            Color = category.Color,
            CreatedAt = category.CreatedAt
        };
    }

    public static Category ToEntity(CreateCategoryDto dto)
    {
        return new Category
        {
            UserId = dto.UserId,
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