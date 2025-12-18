using ECommerce.Business.DTOs.Errors;
using ECommerce.Business.DTOs.ShoppingCart;
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
        [ProducesResponseType(typeof(ShoppingCartDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCart() => Ok(await _cart.GetAsync());

        [HttpPost]
        [EndpointSummary("Update user cart.")]
        [ProducesResponseType(typeof(ShoppingCartDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCart([FromBody] UpdateShoppingCartDto dto)
        {
            var updatedCart = await _cart.UpdateAsync(dto);
            return Ok(updatedCart);
        }

        [HttpDelete]
        [EndpointSummary("Clears user cart.")]
        [ProducesResponseType(typeof(ShoppingCartDto), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ClearCart()
        {
            await _cart.ClearAsync();
            return NoContent();
        }
    }
}
