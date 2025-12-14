namespace ECommerce.Business.DTOs.Orders.Admin
{
    public class AdminOrderDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int ItemsCount { get; set; }
        public DateTime Created { get; set; }
    }
}
