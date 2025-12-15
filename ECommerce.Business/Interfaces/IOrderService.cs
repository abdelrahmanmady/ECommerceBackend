using ECommerce.Business.DTOs.Orders.Admin;
using ECommerce.Business.DTOs.Orders.Profile;
using ECommerce.Business.DTOs.Orders.Store;
using ECommerce.Business.DTOs.Pagination;
using ECommerce.Core.Specifications.Orders;

namespace ECommerce.Business.Interfaces
{
    public interface IOrderService
    {
        Task<PagedResponseDto<AdminOrderDto>> GetAllAdminAsync(AdminOrderSpecParams specParams);
        Task<AdminOrderDetailsDto> GetByIdAdminAsync(int orderId);
        Task<AdminOrderDetailsDto> UpdateAdminAsync(int orderId, AdminUpdateOrderDto dto);
        Task DeleteAdminAsync(int orderId);
        Task<PagedResponseDto<OrderDto>> GetAllAsync(OrderSpecParams specParams);
        Task<OrderDto> CheckoutAsync(CheckoutDto dto);
    }
}
