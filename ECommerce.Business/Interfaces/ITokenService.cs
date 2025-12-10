using ECommerce.Core.Entities;

namespace ECommerce.Business.Interfaces
{
    public interface ITokenService
    {
        string CreateAccessToken(ApplicationUser user, ICollection<string> roles);
        RefreshToken GenerateRefreshToken(string userId, bool rememberMe);
    }
}
