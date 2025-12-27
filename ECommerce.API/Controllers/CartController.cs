using ECommerce.Business.DTOs.Errors;
using ECommerce.Business.DTOs.ShoppingCart.Requests;
using ECommerce.Business.DTOs.ShoppingCart.Responses;
using ECommerce.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Tags("Shopping Cart Managment")]
    public class CartController(ICartService cart) : ControllerBase
    {
        private readonly ICartService _cart = cart;

        [HttpGet]
        [EndpointSummary("Get my cart")]
        [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCart() => Ok(await _cart.GetCartAsync());

        [HttpPost]
        [EndpointSummary("Update user cart.")]
        [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCart([FromBody] UpdateCartRequest updateCartRequest)
        {
            var updatedCart = await _cart.UpdateCartAsync(updateCartRequest);
            return Ok(updatedCart);
        }

        [HttpDelete]
        [EndpointSummary("Clears user cart.")]
        [ProducesResponseType(typeof(CartResponse), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ClearCart()
        {
            await _cart.ClearCartAsync();
            return NoContent();
        }
    }
}
