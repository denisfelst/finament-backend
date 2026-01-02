namespace Finament.Application.DTOs.Categories.Requests;

public interface ICategoryWriteBaseDto
{
    public string Name { get; set; }
    public decimal MonthlyLimit { get; set; }
    public string Color { get; set; }
}