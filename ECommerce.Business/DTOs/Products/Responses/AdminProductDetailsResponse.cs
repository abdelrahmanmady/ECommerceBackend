using ECommerce.Business.DTOs.ProductImages.Responses;

namespace ECommerce.Business.DTOs.Products.Responses
{
    public class AdminProductDetailsResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool IsFeatured { get; set; }
        public IEnumerable<ProductImageDto> Images { get; set; } = [];

    }
}
