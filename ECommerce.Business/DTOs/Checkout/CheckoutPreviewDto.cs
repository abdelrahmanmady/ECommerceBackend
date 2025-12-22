using ECommerce.Business.DTOs.ShoppingCart;

namespace ECommerce.Business.DTOs.Checkout
{
    public class CheckoutPreviewDto
    {
        public decimal Subtotal { get; set; }
        public decimal ShippingFees { get; set; }
        public decimal Taxes { get; set; }
        public decimal Total { get; set; }
        public IEnumerable<CartItemDto> Items { get; set; } = [];
    }
}
