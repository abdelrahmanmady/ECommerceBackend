using AutoMapper;
using AutoMapper.QueryableExtensions;
using ECommerce.Business.DTOs.Dashboard.Responses;
using ECommerce.Business.DTOs.Orders.Responses;
using ECommerce.Business.DTOs.Users.Responses;
using ECommerce.Business.Interfaces;
using ECommerce.Core.Entities;
using ECommerce.Core.Enums;
using ECommerce.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Business.Services
{
    public class DashboardService(AppDbContext context,
        IMapper mapper,
        UserManager<ApplicationUser> userManager) : IDashboardService
    {
        private readonly AppDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<AdminDashboardStatsResponse> GetStatsAsync()
        {
            var now = DateTime.UtcNow;
            var currentRangeStart = now.Date.AddDays(-30);
            var previousRangeStart = now.Date.AddDays(-60);
            var trendStartDate = DateTime.UtcNow.Date.AddDays(-6);
            //Orders Insights

            // TotalOrders
            var totalOrders = await _context.Orders.AsNoTracking().CountAsync();

            //OrdersChangePercentage
            var currentOrdersCount = await _context.Orders
                .AsNoTracking()
                .CountAsync(o => o.Created.Date >= currentRangeStart.Date);
            var previousOrdersCount = await _context.Orders
                .AsNoTracking()
                .CountAsync(o => o.Created.Date >= previousRangeStart.Date && o.Created.Date < currentRangeStart.Date);

            double? ordersChangePercentage = null;
            if (previousOrdersCount > 0)
                ordersChangePercentage = Math.Round(((double)(currentOrdersCount - previousOrdersCount) / previousOrdersCount) * 100);

            //OrdersTrend

            var rawOrdersTrend = await _context.Orders
                .AsNoTracking()
                .Where(o => o.Created.Date >= trendStartDate.Date)
                .OrderBy(o => o.Created.Date)
                .GroupBy(o => o.Created.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToDictionaryAsync(k => k.Date, v => v.Count);

            var ordersTrend = Enumerable.Range(0, 7)
                .Select(offset =>
                {
                    var date = trendStartDate.Date.AddDays(offset);
                    if (rawOrdersTrend.TryGetValue(date, out int count))
                        return count;
                    return 0;
                })
                .ToList();


            //OrdersDistribution
            var rawCounts = await _context.Orders
                .AsNoTracking()
                .GroupBy(o => o.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count);

            var ordersDistribution = Enum.GetValues<OrderStatus>()
                .ToDictionary(status => status, status => 0);

            foreach (var kvp in rawCounts)
            {
                if (ordersDistribution.ContainsKey(kvp.Key))
                    ordersDistribution[kvp.Key] = kvp.Value;
            }

            //RecentOrders
            var recentOrders = await _context.Orders
                .AsNoTracking()
                .OrderByDescending(o => o.Created)
                .Take(5)
                .ProjectTo<AdminOrderSummaryDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            //ProductsInsights

            //TotalProducts(Excluedes Deleted Products)
            var totalProducts = await _context.Products
                .AsNoTracking()
                .CountAsync();

            //RecentProductsCount(Added in the last 7 days)
            var recentProductsCount = await _context.Products
                .AsNoTracking()
                .CountAsync(p => p.Created >= now.AddDays(-7));

            //ProductsTrend
            var rawProductsTrend = await _context.Products
                .AsNoTracking()
                .Where(p => p.Created.Date >= trendStartDate.Date)
                .OrderBy(p => p.Created.Date)
                .GroupBy(p => p.Created.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToDictionaryAsync(k => k.Date, v => v.Count);

            var productsTrend = Enumerable.Range(0, 7)
                .Select(offset =>
                {
                    var date = trendStartDate.Date.AddDays(offset);
                    if (rawProductsTrend.TryGetValue(date, out int count))
                        return count;
                    return 0;
                })
                .ToList();

            //ProductsDistribution
            var productsStockCounts = await _context.Products
                .AsNoTracking()
                .GroupBy(x => 1)
                .Select(g => new
                {
                    OutOfStock = g.Count(p => p.StockQuantity == 0),
                    LowStock = g.Count(p => p.StockQuantity > 0 && p.StockQuantity <= 5),
                    InStock = g.Count(p => p.StockQuantity > 5)
                })
                .OrderBy(x => x.InStock)
                .FirstOrDefaultAsync();
            var productsDistribution = new Dictionary<string, int>
            {
                {"outOfStock",productsStockCounts?.OutOfStock ?? 0 },
                {"lowStock",productsStockCounts?.LowStock ?? 0 },
                {"inStock",productsStockCounts?.InStock ?? 0  }
            };

            //UsersInsights

            //TotalUsers
            var totalUsers = await _context.Users
                .AsNoTracking()
                .CountAsync();

            //RecentUsersCount
            var recentUsersCount = await _context.Users
                .AsNoTracking()
                .CountAsync(u => u.Created >= now.AddDays(-7));

            //UsersTrend
            var rawUsersTrend = await _context.Users
                .AsNoTracking()
                .Where(u => u.Created.Date >= trendStartDate.Date)
                .OrderBy(u => u.Created.Date)
                .GroupBy(u => u.Created.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToDictionaryAsync(k => k.Date, v => v.Count);

            var usersTrend = Enumerable.Range(0, 7)
                .Select(offset =>
                {
                    var date = trendStartDate.Date.AddDays(offset);
                    if (rawUsersTrend.TryGetValue(date, out var count))
                        return count;
                    return 0;
                })
                .ToList();

            //UsersDistribution
            var usersDistribution = await _context.Roles
                .AsNoTracking()
                .Select(r => new
                {
                    RoleName = r.Name,
                    Count = _context.Set<IdentityUserRole<string>>().Count(ur => ur.RoleId == r.Id)
                })
                .ToDictionaryAsync(k => k.RoleName ?? "Unknown", v => v.Count);

            //RecentUsers
            var recentUsers = await _context.Users
                .AsNoTracking()
                .OrderByDescending(u => u.Created)
                .Take(5)
                .Select(u => new AdminUserSummaryDto
                {
                    Id = u.Id,
                    AvatarUrl = u.AvatarUrl,
                    FullName = $"{u.FirstName} {u.LastName}",
                    Email = u.Email!,
                    EmailConfirmed = u.EmailConfirmed,
                    PhoneNumber = u.PhoneNumber,
                    PhoneNumberConfirmed = u.PhoneNumberConfirmed,
                    OrdersCount = u.Orders.Count(),
                    Role = _context.Set<IdentityUserRole<string>>()
                    .Where(ur => ur.UserId == u.Id)
                    .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                    .OrderBy(n => n)
                    .FirstOrDefault() ?? "Unknown",
                    Created = u.Created,
                    Updated = u.Updated,
                    IsDeleted = u.IsDeleted
                })
                .ToListAsync();


            //Revenue Insights

            //Revenue
            var revenue = Math.Round((double)await _context.Orders
                .AsNoTracking()
                .Where(o => o.Status != OrderStatus.Cancelled)
                .SumAsync(o => o.TotalAmount));

            //RevenueChangePercentage
            var currentRevenue = await _context.Orders
                .AsNoTracking()
                .Where(o => o.Created >= currentRangeStart && o.Status != OrderStatus.Cancelled)
                .SumAsync(o => o.TotalAmount);
            var previousRevenue = await _context.Orders
                .AsNoTracking()
                .Where(o => o.Created >= previousRangeStart && o.Created < currentRangeStart && o.Status != OrderStatus.Cancelled)
                .SumAsync(o => o.TotalAmount);

            double? revenueChangePercentage = null;

            if (previousRevenue > 0)
                revenueChangePercentage = Math.Round(((double)(currentRevenue - previousRevenue) / (double)previousRevenue) * 100);

            //RevenuesTrend
            var rawRevenuesTrend = await _context.Orders
                .AsNoTracking()
                .Where(o => o.Created.Date >= trendStartDate.Date && o.Status != OrderStatus.Cancelled)
                .OrderBy(o => o.Created.Date)
                .GroupBy(o => o.Created.Date)
                .Select(g => new { Date = g.Key, DailyRevenue = (int)g.Sum(o => o.TotalAmount) })
                .ToDictionaryAsync(k => k.Date, v => v.DailyRevenue);

            var revenuesTrend = Enumerable.Range(0, 7)
                .Select(offset =>
                {
                    var date = trendStartDate.Date.AddDays(offset);
                    if (rawRevenuesTrend.TryGetValue(date, out int dailyRevenue))
                        return dailyRevenue;
                    return 0;
                })
                .ToList();

            var response = new AdminDashboardStatsResponse
            {
                TotalOrders = totalOrders,
                TotalProducts = totalProducts,
                TotalUsers = totalUsers,
                Revenue = revenue,
                OrdersChangePercentage = ordersChangePercentage,
                RecentProductsCount = recentProductsCount,
                RecentUsersCount = recentUsersCount,
                RevenueChangePercentage = revenueChangePercentage,
                OrdersTrend = ordersTrend,
                ProductsTrend = productsTrend,
                UsersTrend = usersTrend,
                RevenuesTrend = revenuesTrend,
                OrdersDistribution = ordersDistribution,
                ProductsDistribution = productsDistribution,
                UsersDistribution = usersDistribution,
                RecentOrders = recentOrders,
                RecentUsers = recentUsers,
            };

            return response;

        }
    }
}
