namespace ECommerce.Business.DTOs.OrderItems
{
    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ProductThumbnailUrl { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }

    }
}
