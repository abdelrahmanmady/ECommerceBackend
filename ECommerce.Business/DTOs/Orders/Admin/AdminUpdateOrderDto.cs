using ECommerce.Core.Enums;

namespace ECommerce.Business.DTOs.Orders.Admin
{
    public class AdminUpdateOrderDto
    {
        public OrderStatus Status { get; set; }
        public int? ShippingAddressId { get; set; }
    }
}
