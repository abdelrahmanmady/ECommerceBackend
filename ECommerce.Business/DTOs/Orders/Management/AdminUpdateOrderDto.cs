using ECommerce.Core.Enums;

namespace ECommerce.Business.DTOs.Orders.Management
{
    public class AdminUpdateOrderDto
    {
        public OrderStatus Status { get; set; }
        public int AddressId { get; set; }
    }
}
