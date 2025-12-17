using ECommerce.Business.DTOs.Users.Auth;

namespace ECommerce.Business.DTOs.Auth
{
    public class LoginResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public UserSessionDto User { get; set; } = null!;
    }
}
