using ECommerce.Business.DTOs.Checkout.Requests;
using ECommerce.Business.DTOs.Checkout.Responses;
using ECommerce.Core.Enums;

namespace ECommerce.Business.Interfaces
{
    public interface ICheckoutService
    {
        Task<CheckoutPreviewResponse> GetCheckoutPreviewAsync(ShippingMethod shippingMethod);
        Task<OrderConfirmationResponse> CheckoutAsync(CheckoutRequest checkoutRequest);
    }
}
