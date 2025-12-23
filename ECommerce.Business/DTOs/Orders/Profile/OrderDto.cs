using ECommerce.Business.DTOs.OrderItems;
using ECommerce.Business.DTOs.OrderTrackingMilestones;
using ECommerce.Core.Entities;
using ECommerce.Core.Enums;

namespace ECommerce.Business.DTOs.Orders.Profile
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public IEnumerable<OrderItemDto> Items { get; set; } = [];
        public int ItemsCount { get; set; }
        public OrderStatus Status { get; set; }
        public decimal Subtotal { get; set; }
        public decimal ShippingFees { get; set; }
        public decimal Taxes { get; set; }
        public decimal TotalAmount { get; set; }
        public ShippingMethod ShippingMethod { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public OrderAddress ShippingAddress { get; set; } = null!;
        public ICollection<OrderTrackingMilestoneDto> OrderTrackingMilestones { get; set; } = [];


    }
}
