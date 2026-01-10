using AutoMapper;
using AutoMapper.QueryableExtensions;
using ECommerce.Business.DTOs.Auth.Responses;
using ECommerce.Business.DTOs.Pagination;
using ECommerce.Business.DTOs.Users.Requests;
using ECommerce.Business.DTOs.Users.Responses;
using ECommerce.Business.Interfaces;
using ECommerce.Core.Entities;
using ECommerce.Core.Exceptions;
using ECommerce.Core.Specifications.Users;
using ECommerce.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ECommerce.Business.Services
{
    public class UserService(AppDbContext context,
        IMapper mapper,
        ILogger<UserService> logger,
        IHttpContextAccessor httpContext,
        IFileStorageService fileStorageService,
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService) : IUserService
    {
        private readonly AppDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<UserService> _logger = logger;
        private readonly IHttpContextAccessor _httpContext = httpContext;
        private readonly IFileStorageService _fileStorageService = fileStorageService;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ITokenService _tokenService = tokenService;

        //Customers
        public async Task<UserDetailsResponse> GetDetailsAsync()
        {
            var currentUserId = GetCurrentUserId();
            var userDetails = await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == currentUserId)
                .ProjectTo<UserDetailsResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException("User does not exist");
            return userDetails;

        }

        public async Task<UserDetailsResponse> UpdateImageAsync(UploadImageRequest uploadImageRequest)
        {
            var currentUserId = GetCurrentUserId();
            var user = await _context.Users.FindAsync(currentUserId)
                ?? throw new NotFoundException("User does not exist.");

            //remove old image if found
            if (user.AvatarUrl is not null)
            {
                await _fileStorageService.DeleteFileAsync(user.AvatarUrl);
            }

            //upload new image
            var relativePath = await _fileStorageService.SaveFileAsync(uploadImageRequest.File, "users");
            user.AvatarUrl = relativePath;
            user.Updated = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var fileName = Path.GetFileName(relativePath);

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("User {userID} successfully add a new profile image with name {fileName}",
                    currentUserId,
                    fileName);
            }

            return _mapper.Map<UserDetailsResponse>(user);
        }

        public async Task DeleteImageAsync()
        {
            var currentUserId = GetCurrentUserId();
            var user = await _context.Users.FindAsync(currentUserId)
                ?? throw new NotFoundException("User does not exist.");


            if (user.AvatarUrl is null)
                throw new NotFoundException("Profile Image does not exist.");

            var filePath = user.AvatarUrl;
            var fileName = Path.GetFileName(filePath);
            user.AvatarUrl = null;
            await _fileStorageService.DeleteFileAsync(filePath);
            user.Updated = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("User {userID} removed his existing profile image with name {fileName}",
                    currentUserId,
                    fileName);
            }

        }

        public async Task<AuthResponse> UpdatePersonalInfoAsync(UpdateUserRequest updateUserRequest)
        {
            var currentUserId = GetCurrentUserId();
            var user = await _context.Users.FindAsync(currentUserId)
                ?? throw new NotFoundException("User does not exist.");

            // 1. Update Basic Fields
            user.FirstName = updateUserRequest.FirstName;
            user.LastName = updateUserRequest.LastName;
            user.PhoneNumber = updateUserRequest.PhoneNumber;


            if (updateUserRequest.Email != user.Email)
            {
                var emailExists = await _userManager.FindByEmailAsync(updateUserRequest.Email);
                if (emailExists != null && emailExists.Id != currentUserId)
                    throw new ConflictException("Email is already in use.");

                user.Email = updateUserRequest.Email;
            }

            if (updateUserRequest.UserName != user.UserName)
            {
                var userNameExists = await _userManager.FindByNameAsync(updateUserRequest.UserName);
                if (userNameExists != null && userNameExists.Id != currentUserId)
                    throw new ConflictException("Username is already taken.");

                user.UserName = updateUserRequest.UserName;
            }

            var result = await _userManager.UpdateAsync(user);
            user.Updated = DateTime.UtcNow;

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => $"{e.Code}:{e.Description}"));
                throw new ConflictException(errors);
            }


            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _tokenService.CreateAccessToken(user, roles);

            return new AuthResponse
            {
                AccessToken = accessToken,
                UserId = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email!,
                Roles = roles,
                AvatarUrl = user.AvatarUrl,
            };
        }

        public async Task<(AuthResponse, string, DateTime)> UpdatePasswordAsync(UpdatePasswordRequest updatePasswordRequest)
        {
            var currentUserId = GetCurrentUserId();
            var user = await _context.Users.FindAsync(currentUserId)
                ?? throw new NotFoundException("User does not exist.");

            var result = await _userManager.ChangePasswordAsync(user, updatePasswordRequest.OldPassword, updatePasswordRequest.NewPassword);
            user.Updated = DateTime.UtcNow;
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => $"{e.Code}:{e.Description}"));
                throw new BadRequestException(errors);
            }

            var userTokens = await _context.RefreshTokens.Where(t => t.UserId == currentUserId).ToListAsync();
            var existingToken = userTokens.OrderByDescending(t => t.Created).FirstOrDefault();
            bool isLongLived = false;

            if (existingToken != null)
            {
                isLongLived = (existingToken.ExpiresOn - existingToken.Created).TotalDays > 7;
            }

            if (userTokens.Count != 0)
            {
                _context.RefreshTokens.RemoveRange(userTokens);
            }

            var roles = await _userManager.GetRolesAsync(user);

            var accessToken = _tokenService.CreateAccessToken(user, roles);

            var refreshToken = _tokenService.GenerateRefreshToken(currentUserId, isLongLived);

            _context.RefreshTokens.Add(refreshToken);

            await _context.SaveChangesAsync();

            // 5. Create Response Object
            var authResponse = new AuthResponse
            {
                AccessToken = accessToken,
                UserId = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email!,
                Roles = roles,
                AvatarUrl = user.AvatarUrl
            };

            // 6. Return Tuple
            return (authResponse, refreshToken.Token, refreshToken.ExpiresOn);
        }

        //Admin

        public async Task<PagedResponse<AdminUserSummaryDto>> GetAllUsersAdminAsync(AdminUserSpecParams specParams)
        {
            var query = _context.Users.IgnoreQueryFilters().AsNoTracking().Include(u => u.Orders).AsQueryable();

            //Filter
            query = specParams.Status switch
            {
                "active" => query.Where(u => !u.IsDeleted),
                "deleted" => query.Where(u => u.IsDeleted),
                _ => query,
            };

            if (!string.IsNullOrEmpty(specParams.Role))
            {
                query = query.Where(u => _context.Set<IdentityUserRole<string>>()
                .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur, r })
                .Any(x => x.ur.UserId == u.Id && x.r.Name == specParams.Role));
            }

            //Search
            if (!string.IsNullOrEmpty(specParams.Search))
            {
                query = query.Where(u => u.FirstName.Contains(specParams.Search)
                || u.LastName.Contains(specParams.Search)
                || u.Email!.Contains(specParams.Search)
                || (u.PhoneNumber != null && u.PhoneNumber.Contains(specParams.Search)));
            }

            //Sort
            query = specParams.Sort switch
            {
                "createdAsc" => query.OrderBy(u => u.Created),
                "createdDesc" => query.OrderByDescending(u => u.Created),
                "updatedDesc" => query.OrderByDescending(u => u.Updated).ThenByDescending(u => u.Created),
                "ordersDesc" => query.OrderByDescending(u => u.Orders.Count()),
                "nameAsc" => query.OrderBy(u => u.FirstName).ThenBy(u => u.LastName),
                "emailAsc" => query.OrderBy(u => u.Email),
                _ => query.OrderByDescending(u => u.Created)
            };

            var totalCount = await query.CountAsync();
            var rawitems = await query
                .Skip((specParams.PageIndex - 1) * specParams.PageSize)
                .Take(specParams.PageSize)
                .Select(u => new
                {
                    User = u,
                    Role = _context.Set<IdentityUserRole<string>>()
                    .Where(ur => ur.UserId == u.Id)
                    .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                    .OrderBy(n => n)
                    .FirstOrDefault() ?? "Unknown"

                })
                .ToListAsync();

            var items = rawitems.Select(item =>
            {
                var userDto = _mapper.Map<AdminUserSummaryDto>(item.User);
                userDto.Role = item.Role;
                return userDto;
            })
                .ToList();

            return new PagedResponse<AdminUserSummaryDto>
            {
                PageIndex = specParams.PageIndex,
                PageSize = specParams.PageSize,
                TotalCount = totalCount,
                Items = items
            };

        }

        public Task<AdminUserDetailsResponse> GetUserDetailsAdminAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<AdminUserDetailsResponse> UpdateUserRoleAdminAsync(string userId, string role)
        {
            throw new NotImplementedException();
        }

        public Task DeleteUserAdminAsync(string userId)
        {
            throw new NotImplementedException();
        }

        //Helper Methods
        private string GetCurrentUserId()
        {
            var userId = _httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedException("User is not authenticated.");

            return userId;
        }
    }
}
