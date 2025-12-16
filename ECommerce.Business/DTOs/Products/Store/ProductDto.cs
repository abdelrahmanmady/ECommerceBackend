namespace ECommerce.Business.DTOs.Products.Store
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string CategoryBreadcrumb { get; set; } = string.Empty;
        public string BrandedName { get; set; } = string.Empty;
        public decimal Price { get; set; }

    }


}
