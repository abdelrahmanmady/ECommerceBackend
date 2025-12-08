using AutoMapper;
using Microsoft.AspNetCore.Identity;
using MyApp.API.DTOs.Auth;
using MyApp.API.Entities;
using MyApp.API.Interfaces;

namespace MyApp.API.Services
{
    public class AuthService(UserManager<ApplicationUser> userManager, IMapper mapper, ILogger<AuthService> logger) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<AuthService> _logger = logger;

        public async Task<bool> Register(RegisterDto dto)
        {
            return true;
        }
    }
}
