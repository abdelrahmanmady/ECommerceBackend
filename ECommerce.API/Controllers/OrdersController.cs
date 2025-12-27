using ECommerce.Business.DTOs.Errors;
using ECommerce.Business.DTOs.Orders.Requests;
using ECommerce.Business.DTOs.Orders.Responses;
using ECommerce.Business.DTOs.Pagination;
using ECommerce.Business.Interfaces;
using ECommerce.Core.Specifications.Orders;
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
        [ProducesResponseType(typeof(PagedResponse<AdminOrderSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAllOrdersAdmin([FromQuery] AdminOrderSpecParams specParams)
            => Ok(await _orders.GetAllOrdersAdminAsync(specParams));



        [HttpGet("admin/{orderId:int}")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Get order details for an order.")]
        [ProducesResponseType(typeof(OrderDetailsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrderDetailsAdmin([FromRoute] int orderId)
            => Ok(await _orders.GetOrderDetailsAdminAsync(orderId));


        [HttpPut("admin/{orderId:int}")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Update order status , shipping address (if applicable).")]
        [ProducesResponseType(typeof(OrderDetailsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateOrderAdmin([FromRoute] int orderId, [FromBody] UpdateOrderRequest dto)
        {
            var updatedOrder = await _orders.UpdateOrderAdminAsync(orderId, dto);
            return Ok(updatedOrder);
        }

        [HttpDelete("admin/{orderId:int}")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Delete an order.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteOrderAdmin([FromRoute] int orderId)
        {
            await _orders.DeleteOrderAdminAsync(orderId);
            return NoContent();
        }

        [HttpGet]
        [EndpointSummary("Get all orders of logged in user.")]
        [ProducesResponseType(typeof(PagedResponse<OrderSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllOrders([FromQuery] OrderSpecParams specParams)
            => Ok(await _orders.GetAllOrdersAsync(specParams));

    }
}
