namespace ECommerce.Business.DTOs.Categories.Responses
{
    public class CategorySummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? ParentId { get; set; }
        public List<CategorySummaryDto> Subcategories { get; set; } = [];
    }
}
