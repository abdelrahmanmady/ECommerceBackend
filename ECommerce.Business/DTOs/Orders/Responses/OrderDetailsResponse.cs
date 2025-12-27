using ECommerce.Business.DTOs.OrderItems;
using ECommerce.Core.Entities;
using ECommerce.Core.Enums;

namespace ECommerce.Business.DTOs.Orders.Responses
{
    public class OrderDetailsResponse
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public OrderStatus Status { get; set; }
        public OrderAddress ShippingAddress { get; set; } = null!;
        public ICollection<OrderItemDto> Items { get; set; } = [];

    }
}
