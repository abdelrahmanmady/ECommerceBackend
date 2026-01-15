using ECommerce.Business.DTOs.Dashboard.Responses;
using ECommerce.Business.DTOs.Errors;
using ECommerce.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController(IDashboardService dashboard) : ControllerBase
    {
        private readonly IDashboardService _dashboard = dashboard;

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("admin")]
        [EndpointSummary("Retrieves Summary insights for the dashboard home page.")]
        [ProducesResponseType(typeof(AdminDashboardStatsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetStats()
        {
            var response = await _dashboard.GetStatsAsync();
            return Ok(response);
        }

    }
}
