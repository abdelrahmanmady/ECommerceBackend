namespace ECommerce.Business.DTOs.Products.Store
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }


}
