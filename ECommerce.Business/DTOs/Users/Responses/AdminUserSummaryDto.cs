namespace ECommerce.Business.DTOs.Users.Responses
{
    public class AdminUserSummaryDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
        public string? PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public int OrdersCount { get; set; }
        public string Role { get; set; } = string.Empty;
        public DateTime Created { get; set; }
        public bool IsDeleted { get; set; }


    }
}
