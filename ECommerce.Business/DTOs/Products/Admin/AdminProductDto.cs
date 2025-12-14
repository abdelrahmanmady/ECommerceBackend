using ECommerce.Business.DTOs.Products.Store;

namespace ECommerce.Business.DTOs.Products.Admin
{
    public class AdminProductDto : ProductDto
    {
        public string? Description { get; set; }
        public int StockQuantity { get; set; }
        public bool InStock => (StockQuantity > 0);
        public DateTime Created { get; set; }

    }
}
