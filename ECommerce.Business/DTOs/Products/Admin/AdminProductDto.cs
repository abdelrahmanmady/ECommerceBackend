namespace ECommerce.Business.DTOs.Products.Admin
{
    public class AdminProductDto
    {
        public int Id { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool InStock { get; set; }
        public bool IsFeatured { get; set; }
        public DateTime Created { get; set; }

    }
}
