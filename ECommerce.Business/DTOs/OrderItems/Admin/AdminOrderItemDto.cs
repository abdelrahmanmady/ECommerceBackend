namespace ECommerce.Business.DTOs.OrderItems.Admin
{
    public class AdminOrderItemDto
    {
        public int ProductId { get; set; } //map
        public string ProductName { get; set; } = string.Empty; //map
        public string? ProductThumbnailUrl { get; set; } //map
        public int Qunatity { get; set; }
        public decimal TotalPrice { get; set; }

    }
}
