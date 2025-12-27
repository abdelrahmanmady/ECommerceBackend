using ECommerce.Business.DTOs.Checkout.Requests;
using ECommerce.Business.DTOs.Checkout.Responses;
using ECommerce.Business.DTOs.Errors;
using ECommerce.Business.DTOs.Orders.Responses;
using ECommerce.Business.Interfaces;
using ECommerce.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CheckoutController(ICheckoutService checkout) : ControllerBase
    {
        private readonly ICheckoutService _checkout = checkout;

        [HttpGet("preview")]
        [EndpointSummary("Returns an order summary before checkout.")]
        [ProducesResponseType(typeof(CheckoutPreviewResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]

        public async Task<IActionResult> GetCheckoutPreview([FromQuery] ShippingMethod shippingMethod)
        {
            var result = await _checkout.GetCheckoutPreviewAsync(shippingMethod);
            return Ok(result);
        }

        [HttpPost]
        [EndpointSummary("Checkout order.")]
        [ProducesResponseType(typeof(OrderSummaryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CheckoutOrder([FromBody] CheckoutRequest checkoutRequest)
        {
            var orderCreated = await _checkout.CheckoutAsync(checkoutRequest);
            return Ok(orderCreated);
        }

    }
}
