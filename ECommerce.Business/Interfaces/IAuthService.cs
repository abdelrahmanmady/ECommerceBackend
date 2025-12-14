using ECommerce.Business.DTOs.Auth;
using ECommerce.Business.DTOs.Users.Auth;

namespace ECommerce.Business.Interfaces
{
    public interface IAuthService
    {
        Task<UserSessionDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<AuthResponseDto> RefreshTokenAsync(string token);
        Task RevokeTokenAsync(string token);
    }
}
