namespace ECommerce.Business.DTOs.Products.Responses
{
    public class ProductSummaryDto // Output
    {
        public int Id { get; set; }
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string BrandedName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public List<BreadcrumbLink> CategoryBreadcrumbLinks { get; set; } = [];

    }


}
