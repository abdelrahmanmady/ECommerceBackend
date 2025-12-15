using ECommerce.Core.Enums;

namespace ECommerce.Business.DTOs.Orders.Store
{
    public class CheckoutDto
    {
        public int ShippingAddressId { get; set; }
        public ShippingMethod ShippingMethod { get; set; }
    }
}
