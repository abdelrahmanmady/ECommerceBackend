using ECommerce.Core.Enums;

namespace ECommerce.Business.DTOs.Orders.Responses
{
    public class AdminOrderSummaryDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public int ItemsCount { get; set; }
        public DateTime Created { get; set; }
    }
}
