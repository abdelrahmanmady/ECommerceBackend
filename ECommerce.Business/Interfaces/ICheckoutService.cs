using ECommerce.Business.DTOs.Checkout;
using ECommerce.Business.DTOs.Orders.Profile;
using ECommerce.Core.Enums;

namespace ECommerce.Business.Interfaces
{
    public interface ICheckoutService
    {
        Task<CheckoutPreviewDto> GetCheckoutPreviewAsync(ShippingMethod shippingMethod);
        Task<OrderDto> CheckoutAsync(CheckoutDto dto);
    }
}
