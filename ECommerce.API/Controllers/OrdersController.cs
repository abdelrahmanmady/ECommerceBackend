using ECommerce.Business.DTOs.Errors;
using ECommerce.Business.DTOs.Orders.Admin;
using ECommerce.Business.DTOs.Pagination;
using ECommerce.Business.Interfaces;
using ECommerce.Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Tags("Orders Management")]
    public class OrdersController(IOrderService orders) : ControllerBase
    {
        private readonly IOrderService _orders = orders;


        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Get all orders for admin dashboard.")]
        [EndpointDescription("Retrieves all placed orders for admin dashboard.")]
        [ProducesResponseType(typeof(PagedResponseDto<AdminOrderDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAllAdmin([FromQuery] AdminOrderSpecParams specParams)
            => Ok(await _orders.GetAllAdminAsync(specParams));



        [HttpGet("admin/{orderId:int}")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Get order details for an order.")]
        [EndpointDescription("Retrieves a specific order for admin dashboard.")]
        [ProducesResponseType(typeof(AdminOrderDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByIdAdmin([FromRoute] int orderId)
            => Ok(await _orders.GetByIdAdminAsync(orderId));


        [HttpPut("admin/{orderId:int}")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Update order status , shipping address (if applicable).")]
        [EndpointDescription("Updates the status of an order (e.g., to Shipped or Delivered), Shipping Address if order is pending or processing.")]
        [ProducesResponseType(typeof(AdminOrderDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateOrderAdmin([FromRoute] int orderId, [FromBody] AdminUpdateOrderDto dto)
        {
            var updatedOrder = await _orders.UpdateAdminAsync(orderId, dto);
            return Ok(updatedOrder);
        }

        [HttpDelete("admin/{orderId:int}")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Delete an order.")]
        [EndpointDescription("Permanently deletes an order with its orderitems.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteOrderAdmin([FromRoute] int orderId)
        {
            await _orders.DeleteAdminAsync(orderId);
            return NoContent();
        }

        //[HttpPost("checkout")]
        //[EndpointSummary("Checkout Cart")]
        //[ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
        //[ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        //public async Task<IActionResult> Checkout([FromBody] CheckoutDto dto)
        //{
        //    var createdOrder = await _orders.CheckoutAsync(dto);
        //    return CreatedAtAction(nameof(GetById), new { id = createdOrder.Id }, createdOrder);
        //}
    }
}
