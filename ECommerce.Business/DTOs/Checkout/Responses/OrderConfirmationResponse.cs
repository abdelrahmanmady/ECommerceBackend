using ECommerce.Business.DTOs.OrderItems;
using ECommerce.Core.Entities;
using ECommerce.Core.Enums;

namespace ECommerce.Business.DTOs.Checkout.Responses
{
    public class OrderConfirmationResponse
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public decimal Subtotal { get; set; }
        public decimal ShippingFees { get; set; }
        public decimal Taxes { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime EstimatedDeliveryDateStart { get; set; }
        public DateTime EstimatedDeliveryDateEnd { get; set; }
        public ShippingMethod ShippingMethod { get; set; }
        public OrderAddress ShippingAddress { get; set; } = null!;
        public string UserEmail { get; set; } = string.Empty;
        public PaymentMethod PaymentMethod { get; set; }
        public int ItemsCount { get; set; }
        public IEnumerable<OrderItemDto> Items { get; set; } = [];

    }
}
