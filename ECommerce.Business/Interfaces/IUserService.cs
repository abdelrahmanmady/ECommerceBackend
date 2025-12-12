using ECommerce.Business.DTOs.Pagination;
using ECommerce.Business.DTOs.Users;
using ECommerce.Core.Specifications;

namespace ECommerce.Business.Interfaces
{
    public interface IUserService
    {
        Task<UserDetailsDto> GetCurrentUserProfileAsync();
        Task<UserDetailsDto> UpdateCurrentUserProfileAsync(UpdateUserDto dto);

        // Admin Methods
        Task<PagedResponseDto<UserManagementDto>> GetAllUsersAsync(UserSpecParams specParams);
        Task<UserDetailsDto> GetUserByIdAsync(string id);
        Task DeleteUserAsync(string id);
    }
}
