using ECommerce.Core.Enums;

namespace ECommerce.Business.DTOs.Checkout
{
    public class CheckoutDto
    {
        public int ShippingAddressId { get; set; }
        public ShippingMethod ShippingMethod { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}
