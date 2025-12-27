using System.ComponentModel.DataAnnotations;

namespace ECommerce.Business.DTOs.Products.Requests
{
    public class UpdateProductRequest
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }

        [Range(0.1, double.MaxValue, ErrorMessage = "Price Must be greater than zero.")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock must be greater than or equal to zero")]
        public int StockQuantity { get; set; }
        public bool IsFeatured { get; set; }
    }


}
