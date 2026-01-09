using ECommerce.Business.DTOs.Dashboard.Responses;

namespace ECommerce.Business.Interfaces
{
    public interface IDashboardService
    {
        Task<AdminDashboardStatsResponse> GetStatsAsync();

    }
}
