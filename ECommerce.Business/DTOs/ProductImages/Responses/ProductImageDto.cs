namespace ECommerce.Business.DTOs.ProductImages.Responses
{
    public class ProductImageDto // Output
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsMain { get; set; }
    }
}
