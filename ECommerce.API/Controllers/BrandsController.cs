using ECommerce.Business.DTOs.Brands.Requests;
using ECommerce.Business.DTOs.Brands.Responses;
using ECommerce.Business.DTOs.Errors;
using ECommerce.Business.DTOs.Pagination;
using ECommerce.Business.Interfaces;
using ECommerce.Core.Specifications.Brands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Tags("Brands Management")]
    public class BrandsController(IBrandService brands) : ControllerBase
    {
        private readonly IBrandService _brands = brands;

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Get all brands with paging and search support.")]
        [ProducesResponseType(typeof(PagedResponse<AdminBrandSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAllBrandsAsmin([FromQuery] AdminBrandSpecParams specParams)
        {
            var brands = await _brands.GetAllBrandsAdminAsync(specParams);
            return Ok(brands);
        }

        [HttpGet("admin/{brandId:int}")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Get brand details.")]
        [ProducesResponseType(typeof(BrandDetailsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBrandDetailsAdmin([FromRoute] int brandId)
        {
            var brand = await _brands.GetBrandDetailsAdminAsync(brandId);
            return Ok(brand);
        }

        [HttpPost("admin")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Create a new brand.")]
        [ProducesResponseType(typeof(BrandDetailsResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateBrandAdmin([FromBody] CreateBrandRequest createBrandRequest)
        {
            var brandCreated = await _brands.CreateBrandAdminAsync(createBrandRequest);
            return CreatedAtAction(nameof(GetBrandDetailsAdmin), new { brandCreated.Id }, brandCreated);
        }


        [HttpPost("admin/{brandId:int}")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Update existing brand.")]
        [ProducesResponseType(typeof(BrandDetailsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateBrandAdmin([FromRoute] int brandId, [FromBody] UpdateBrandRequest updateBrandRequest)
        {
            var brandUpdated = await _brands.UpdateBrandAdminAsync(brandId, updateBrandRequest);
            return Ok(brandUpdated);
        }

        [HttpDelete("admin/{brandId:int}")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Delete existing brand if no products are referencing it.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteBrandAdmin([FromRoute] int brandId)
        {
            await _brands.DeleteBrandAdminAsync(brandId);
            return NoContent();
        }

        [HttpGet]
        [EndpointSummary("Get all brands.")]
        [ProducesResponseType(typeof(IEnumerable<BrandSummaryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllBrands()
        {
            var brands = await _brands.GetAllBrandsAsync();
            return Ok(brands);
        }

    }
}
