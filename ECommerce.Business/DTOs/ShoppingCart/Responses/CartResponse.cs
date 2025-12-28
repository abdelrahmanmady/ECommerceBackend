using ECommerce.Business.DTOs.OrderItems;

namespace ECommerce.Business.DTOs.ShoppingCart.Responses
{
    public class CartResponse
    {
        public ICollection<OrderItemDto> Items { get; set; } = [];
        public decimal CartTotal { get; set; }
        public IEnumerable<string> Warnings { get; set; } = [];

    }
}
