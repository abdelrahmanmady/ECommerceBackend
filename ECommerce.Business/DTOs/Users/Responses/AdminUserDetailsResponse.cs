using ECommerce.Business.DTOs.Addresses.Responses;

namespace ECommerce.Business.DTOs.Users.Responses
{
    public class AdminUserDetailsResponse
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
        public string? PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public string Role { get; set; } = string.Empty;
        public int OrdersCount { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public IEnumerable<AddressSummaryDto> Addresses { get; set; } = [];
    }
}
