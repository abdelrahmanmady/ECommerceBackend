using ECommerce.Business.DTOs.Orders.Admin;
using ECommerce.Business.DTOs.Orders.Profile;
using ECommerce.Business.DTOs.Orders.Store;
using ECommerce.Business.DTOs.Pagination;
using ECommerce.Core.Specifications.Orders;

namespace ECommerce.Business.Interfaces
{
    public interface IOrderService
    {
        Task<PagedResponseDto<AdminOrderDto>> GetAllOrdersAdminAsync(AdminOrderSpecParams specParams);
        Task<AdminOrderDetailsDto> GetOrderDetailsAdminAsync(int orderId);
        Task<AdminOrderDetailsDto> UpdateOrderAdminAsync(int orderId, AdminUpdateOrderDto dto);
        Task DeleteOrderAdminAsync(int orderId);
        Task<PagedResponseDto<OrderDto>> GetAllOrdersAsync(OrderSpecParams specParams);
        Task<OrderDto> CheckoutAsync(CheckoutDto dto);
    }
}
