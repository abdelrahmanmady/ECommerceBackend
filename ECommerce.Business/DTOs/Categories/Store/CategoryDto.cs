namespace ECommerce.Business.DTOs.Categories.Store
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? ParentId { get; set; }
        public List<CategoryDto> Subcategories { get; set; } = [];
    }
}
