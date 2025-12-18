namespace ECommerce.Business.DTOs.ShoppingCart
{
    public class CartItemDto
    {
        public string? ThumbnailUrl { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public string BrandedName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total => Price * Quantity;
    }
}
