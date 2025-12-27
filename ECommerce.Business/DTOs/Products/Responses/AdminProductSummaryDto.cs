namespace ECommerce.Business.DTOs.Products.Responses
{
    public class AdminProductSummaryDto
    {
        public int Id { get; set; }
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool InStock { get; set; }
        public bool IsFeatured { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

    }
}
