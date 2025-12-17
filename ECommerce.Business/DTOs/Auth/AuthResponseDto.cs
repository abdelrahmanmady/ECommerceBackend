using ECommerce.Business.DTOs.Users.Auth;

namespace ECommerce.Business.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiration { get; set; }
        public UserSessionDto User { get; set; } = null!;
    }
}
