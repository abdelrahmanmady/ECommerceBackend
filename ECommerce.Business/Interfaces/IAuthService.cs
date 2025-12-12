using ECommerce.Business.DTOs.Auth;

namespace ECommerce.Business.Interfaces
{
    public interface IAuthService
    {
        Task<RegisterResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<AuthResponseDto> RefreshTokenAsync(string token);
        Task RevokeTokenAsync(string token);
    }
}
