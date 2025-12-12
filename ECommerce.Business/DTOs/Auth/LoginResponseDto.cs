using ECommerce.Business.DTOs.Users;

namespace ECommerce.Business.DTOs.Auth
{
    public class LoginResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public UserDetailsDto User { get; set; } = null!;
    }
}
