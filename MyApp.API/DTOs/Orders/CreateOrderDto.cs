using MyApp.API.DTOs.OrderItems;

namespace MyApp.API.DTOs.Orders
{
    public class CreateOrderDto
    {
        public ICollection<CreateOrderItemDto> Items { get; set; } = null!;

    }
}
