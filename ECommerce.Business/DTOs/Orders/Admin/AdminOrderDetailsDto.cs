using ECommerce.Business.DTOs.OrderItems.Admin;
using ECommerce.Core.Entities;

namespace ECommerce.Business.DTOs.Orders.Admin
{
    public class AdminOrderDetailsDto : AdminOrderDto
    {
        public DateTime Updated { get; set; }
        public OrderAddress ShippingAddress { get; set; } = null!;
        public ICollection<AdminOrderItemDto> Items { get; set; } = [];
    }
}
