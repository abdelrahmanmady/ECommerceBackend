using ECommerce.Business.DTOs.OrderItems;

namespace ECommerce.Business.DTOs.Orders
{
    public class CreateOrderDto
    {
        public int ShippingAddressId { get; set; }
        public ICollection<CreateOrderItemDto> Items { get; set; } = null!;

    }
}
