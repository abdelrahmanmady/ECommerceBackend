using ECommerce.Business.DTOs.Checkout;
using ECommerce.Business.DTOs.Errors;
using ECommerce.Business.DTOs.Orders.Profile;
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
        [ProducesResponseType(typeof(CheckoutPreviewDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCheckoutPreview([FromQuery] ShippingMethod shippingMethod)
        {
            var result = await _checkout.GetCheckoutPreviewAsync(shippingMethod);
            return Ok(result);
        }

        [HttpPost]
        [EndpointSummary("Checkout order.")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CheckoutOrder([FromBody] CheckoutDto dto)
        {
            var orderCreated = await _checkout.CheckoutAsync(dto);
            return Ok(orderCreated);
        }

    }
}
