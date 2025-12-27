using ECommerce.Business.DTOs.Products.Responses;

namespace ECommerce.Business.DTOs.Brands.Responses
{
    public class BrandDetailsResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public IEnumerable<AdminProductSummaryDto> Products { get; set; } = [];
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }

}
