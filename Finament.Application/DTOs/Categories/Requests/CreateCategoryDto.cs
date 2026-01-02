namespace Finament.Application.DTOs.Categories.Requests;

public class CreateCategoryDto: ICategoryWriteBaseDto
{
    public required int UserId { get; set; }
    public required string Name { get; set; }
    public required decimal MonthlyLimit { get; set; }
    public required string Color { get; set; }
}