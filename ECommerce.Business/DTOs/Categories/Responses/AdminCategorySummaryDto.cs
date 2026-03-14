namespace ECommerce.Business.DTOs.Categories.Responses
{
    public class AdminCategorySummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? HierarchyPath { get; set; }
        public bool IsLeaf { get; set; }
        public int ChildrenCount { get; set; }
        public DateTime Created { get; set; }
    }
}
