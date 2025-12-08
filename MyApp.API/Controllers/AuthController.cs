using Microsoft.AspNetCore.Mvc;
using MyApp.API.DTOs.Auth;
using MyApp.API.Interfaces;

namespace MyApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            await _authService.Register(dto);
            return Ok();
        }
    }
}
