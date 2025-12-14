using ECommerce.Business.DTOs.Orders;
using ECommerce.Business.DTOs.Orders.Admin;
using ECommerce.Business.DTOs.Orders.Management;
using ECommerce.Business.DTOs.Pagination;
using ECommerce.Core.Specifications;

namespace ECommerce.Business.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetOrdersForCustomerAsync();
        Task<PagedResponseDto<AdminOrderDto>> GetOrdersForAdminAsync(AdminOrderSpecParams specParams);
        Task<OrderDto> GetByIdCustomerAsync(int id);
        Task<AdminOrderDetailsDto> GetByIdAdminAsync(int id);
        Task<AdminOrderDetailsDto> UpdateOrderAdminAsync(int id, AdminUpdateOrderDto dto);
        Task DeleteAsync(int id);
        Task<OrderDto> CheckoutAsync(CheckoutDto dto);
    }
}
