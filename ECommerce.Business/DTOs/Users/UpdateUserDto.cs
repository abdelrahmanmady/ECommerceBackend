using System.ComponentModel.DataAnnotations;

namespace ECommerce.Business.DTOs.Users
{
    public class UpdateUserDto
    {
        public string UserName { get; set; } = null!;

        [Phone]
        public string? PhoneNumber { get; set; }
    }
}
