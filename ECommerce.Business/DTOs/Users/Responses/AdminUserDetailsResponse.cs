using ECommerce.Business.DTOs.Addresses.Responses;
using ECommerce.Business.DTOs.Orders.Responses;
using ECommerce.Business.DTOs.RefreshTokens.Responses;

namespace ECommerce.Business.DTOs.Users.Responses
{
    public class AdminUserDetailsResponse
    {
        //UserInformation
        public string? AvatarUrl { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
        public string? PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }

        //Account Information
        public string Id { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string AccountStatus { get; set; } = string.Empty;
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        //Metrics
        public int AddressesCount { get; set; }
        public int ReviewsCount { get; set; }
        public int OrdersCount { get; set; }
        public decimal TotalSpent { get; set; }
        public IEnumerable<LoginSessionDto> LoginSessions { get; set; } = [];
        public IEnumerable<AddressSummaryDto> SavedAddresses { get; set; } = [];
        public IEnumerable<AdminOrderSummaryDto> RecentOrders { get; set; } = [];

    }
}
