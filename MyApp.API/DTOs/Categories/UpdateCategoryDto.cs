namespace MyApp.API.DTOs.Categories
{
    public class UpdateCategoryDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}
