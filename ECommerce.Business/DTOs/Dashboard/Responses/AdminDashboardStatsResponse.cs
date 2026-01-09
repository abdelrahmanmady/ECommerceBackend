using ECommerce.Business.DTOs.Orders.Responses;
using ECommerce.Business.DTOs.Users.Responses;
using ECommerce.Core.Enums;

namespace ECommerce.Business.DTOs.Dashboard.Responses
{
    public class AdminDashboardStatsResponse
    {
        public int TotalOrders { get; set; }
        public int TotalProducts { get; set; }
        public int TotalUsers { get; set; }
        public double Revenue { get; set; }
        public double? OrdersChangePercentage { get; set; }
        public int RecentProductsCount { get; set; }
        public int RecentUsersCount { get; set; }
        public double? RevenueChangePercentage { get; set; }
        public IEnumerable<int> OrdersTrend { get; set; } = [];
        public IEnumerable<int> ProductsTrend { get; set; } = [];
        public IEnumerable<int> UsersTrend { get; set; } = [];
        public IEnumerable<int> RevenuesTrend { get; set; } = [];
        public Dictionary<OrderStatus, int> OrdersDistribution { get; set; } = [];
        public Dictionary<string, int> ProductsDistribution { get; set; } = [];
        public Dictionary<string, int> UsersDistribution { get; set; } = [];
        public IEnumerable<AdminOrderSummaryDto> RecentOrders { get; set; } = [];
        public IEnumerable<AdminUserSummaryDto> RecentUsers { get; set; } = [];
    }
}
