using ECommerce.Business.DTOs.OrderItems;

namespace ECommerce.Business.DTOs.Checkout
{
    public class CheckoutPreviewDto
    {
        public decimal Subtotal { get; set; }
        public decimal ShippingFees { get; set; }
        public decimal Taxes { get; set; }
        public decimal Total { get; set; }
        public DateTime EstimatedDeliveryDateStart { get; set; }
        public DateTime EstimatedDeliveryDateEnd { get; set; }
        public IEnumerable<OrderItemDto> Items { get; set; } = [];
    }
}
