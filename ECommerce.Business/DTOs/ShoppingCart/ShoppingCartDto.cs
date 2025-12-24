using ECommerce.Business.DTOs.OrderItems;

namespace ECommerce.Business.DTOs.ShoppingCart
{
    public class ShoppingCartDto
    {
        public ICollection<OrderItemDto> Items { get; set; } = [];
        public decimal Total { get; set; }

    }
}
