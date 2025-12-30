using ECommerce.Business.DTOs.ProductAttribute;
using ECommerce.Business.DTOs.ProductImages.Responses;
using ECommerce.Core.Enums;

namespace ECommerce.Business.DTOs.Products.Responses
{
    public class ProductDetailsResponse // Output
    {
        public int Id { get; set; }
        public IEnumerable<ProductImageDto> Images { get; set; } = [];
        public string Name { get; set; } = string.Empty;
        public int BrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? OverviewHeadline { get; set; }
        public string OverviewDescription { get; set; } = string.Empty;
        public string CompositionText { get; set; } = string.Empty;
        public bool IsSustainable { get; set; }
        public IEnumerable<CareInstructionType> CareInstructions { get; set; } = [];
        public IEnumerable<string> Features { get; set; } = [];
        public IEnumerable<ProductAttributeDto> Attributes { get; set; } = [];
    }
}
