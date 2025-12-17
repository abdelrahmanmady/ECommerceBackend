using ECommerce.Business.DTOs.ProductImages;

namespace ECommerce.Business.DTOs.Products.Store
{
    public class ProductDetailsDto
    {
        public int Id { get; set; }
        public IEnumerable<ProductImageDto> Images { get; set; } = [];
        public string Name { get; set; } = string.Empty;
        public int BrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? Description { get; set; }

    }
}
