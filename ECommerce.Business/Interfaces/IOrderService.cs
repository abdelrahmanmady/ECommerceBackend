using ECommerce.Business.DTOs.Orders.Requests;
using ECommerce.Business.DTOs.Orders.Responses;
using ECommerce.Business.DTOs.Pagination;
using ECommerce.Core.Specifications.Orders;

namespace ECommerce.Business.Interfaces
{
    public interface IOrderService
    {
        Task<PagedResponse<AdminOrderSummaryDto>> GetAllOrdersAdminAsync(AdminOrderSpecParams specParams);
        Task<OrderDetailsResponse> GetOrderDetailsAdminAsync(int orderId);
        Task<OrderDetailsResponse> UpdateOrderAdminAsync(int orderId, UpdateOrderRequest updateOrderRequest);
        Task DeleteOrderAdminAsync(int orderId);
        Task<PagedResponse<OrderSummaryDto>> GetAllOrdersAsync(OrderSpecParams specParams);

    }
}
