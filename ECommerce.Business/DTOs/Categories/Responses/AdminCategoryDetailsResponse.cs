using ECommerce.Business.DTOs.Breadcrumb;
using ECommerce.Business.DTOs.Products.Responses;

namespace ECommerce.Business.DTOs.Categories.Responses
{
    public class AdminCategoryDetailsResponse
    {
        public int Id { get; set; }
        public bool IsLeaf { get; set; }
        public List<BreadcrumbLink> HierarchyBreadcrumb { get; set; } = [];
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? ParentId { get; set; }
        public string ParentName { get; set; } = string.Empty;
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public IEnumerable<AdminCategorySummaryDto> Subcategories { get; set; } = [];
        public int ProductsCount { get; set; }
        public IEnumerable<AdminProductSummaryDto> RecentProducts { get; set; } = [];

    }
}
