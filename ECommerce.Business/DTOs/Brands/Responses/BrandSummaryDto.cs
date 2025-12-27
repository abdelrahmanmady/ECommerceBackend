namespace ECommerce.Business.DTOs.Brands.Responses
{
    public class BrandSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ProductsCount { get; set; }
    }
}
