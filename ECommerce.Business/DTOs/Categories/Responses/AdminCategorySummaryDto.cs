namespace ECommerce.Business.DTOs.Categories.Responses
{
    public class AdminCategorySummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ParentCategoryName { get; set; } = string.Empty;
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string PathFromRoot { get; set; } = string.Empty;
    }
}
