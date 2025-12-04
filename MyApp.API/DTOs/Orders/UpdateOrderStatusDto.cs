using MyApp.API.Enums;

namespace MyApp.API.DTOs.Orders
{
    public class UpdateOrderStatusDto
    {
        public OrderStatus Status { get; set; }
    }
}
