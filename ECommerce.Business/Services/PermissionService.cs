using ECommerce.Business.Interfaces;
using ECommerce.Core.Entities;
using ECommerce.Core.Exceptions;
using ECommerce.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ECommerce.Business.Services
{
    public class PermissionService(
        AppDbContext context,
        UserManager<ApplicationUser> userManager,
        IHttpContextAccessor httpContext) : IPermissionService
    {
        private readonly AppDbContext _context = context;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IHttpContextAccessor _httpContext = httpContext;
        public async Task<bool> CanManageUserAsync(string targetUserId)
        {
            var currentUserId = GetCurrentUserId();

            if (currentUserId == targetUserId)
                return false;

            var currentUser = await _userManager.FindByIdAsync(currentUserId)
                ?? throw new NotFoundException("Logged in User does not exist.");

            var targetUser = await _context.Users
                .AsNoTracking()
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == targetUserId)
                ?? throw new NotFoundException("Target User does not exist.");

            var currentUserRoles = await _userManager.GetRolesAsync(currentUser);
            var targetUserRoles = await _userManager.GetRolesAsync(targetUser);

            if (currentUserRoles.Contains("SuperAdmin"))
                return true;
            if (currentUserRoles.Contains("Admin"))
            {
                if (targetUserRoles.Contains("SuperAdmin") || targetUserRoles.Contains("Admin"))
                    return false;
                else
                    return true;
            }
            return false;
        }

        //Helper Methods
        private string GetCurrentUserId()
        {
            var userId = _httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedException("User is not authenticated.");

            else if (_context.Users.IgnoreQueryFilters().Any(u => u.Id == userId && u.IsDeleted))
                throw new UnauthorizedException("User is no longer active.");

            return userId;
        }

    }
}
