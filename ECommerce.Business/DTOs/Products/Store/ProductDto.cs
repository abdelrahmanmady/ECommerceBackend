namespace ECommerce.Business.DTOs.Products.Store
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string BrandedName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public List<BreadcrumbLink> CategoryBreadcrumbLinks { get; set; } = [];

    }


}
