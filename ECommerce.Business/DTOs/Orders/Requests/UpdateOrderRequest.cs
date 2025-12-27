using ECommerce.Core.Enums;

namespace ECommerce.Business.DTOs.Orders.Requests
{
    public class UpdateOrderRequest
    {
        public OrderStatus Status { get; set; }
        public int? ShippingAddressId { get; set; }
    }
}
