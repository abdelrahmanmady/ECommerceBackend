namespace ECommerce.Business.DTOs.ShoppingCart
{
    public class CartItemDto
    {
        public int ProductId { get; set; }
        public string ProductThumbnailUrl { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal ProductPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Total => ProductPrice * Quantity;
    }
}
