namespace ECommerce.Business.DTOs.Categories.Admin
{
    public class AdminCategoryDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? ParentId { get; set; }
        public IEnumerable<string> SubcategoriesNames { get; set; } = [];
        public string PathFromRoot { get; set; } = string.Empty;
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}
