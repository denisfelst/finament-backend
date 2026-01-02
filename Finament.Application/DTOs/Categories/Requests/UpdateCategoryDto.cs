namespace Finament.Application.DTOs.Categories.Requests;

public class UpdateCategoryDto: ICategoryWriteBaseDto
{
    public required string Name { get; set; }
    public required decimal MonthlyLimit { get; set; }
    public required string Color { get; set; }
}