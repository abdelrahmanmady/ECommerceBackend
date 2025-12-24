using ECommerce.Business.DTOs.Checkout;
using ECommerce.Core.Enums;

namespace ECommerce.Business.Interfaces
{
    public interface ICheckoutService
    {
        Task<CheckoutPreviewDto> GetCheckoutPreviewAsync(ShippingMethod shippingMethod);
        Task<OrderConfirmationDto> CheckoutAsync(CheckoutDto dto);
    }
}
