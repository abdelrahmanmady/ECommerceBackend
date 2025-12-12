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
        public async Task<IActionResult> Get() => Ok(await _cart.GetAsync());

        [HttpPost("items/{productId:int}")]
        [EndpointSummary("Add item to cart")]
        [EndpointDescription("Adds a product to the cart. If it exists, increments quantity by 1. Checks stock.")]
        [ProducesResponseType(typeof(ShoppingCartDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddItem([FromRoute] int productId) => Ok(await _cart.AddItemAsync(productId));

        [HttpDelete("items/{productId:int}")]
        [EndpointSummary("Remove item")]
        [ProducesResponseType(typeof(ShoppingCartDto), StatusCodes.Status200OK)] // Returns updated cart
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveItem([FromRoute] int productId) => Ok(await _cart.RemoveItemAsync(productId));

        [HttpDelete]
        [EndpointSummary("Clear cart")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ClearCart()
        {
            await _cart.ClearAsync();
            return NoContent();
        }

    }
}
