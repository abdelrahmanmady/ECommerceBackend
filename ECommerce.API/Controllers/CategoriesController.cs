using ECommerce.Business.DTOs.Categories.Admin;
using ECommerce.Business.DTOs.Categories.Store;
using ECommerce.Business.DTOs.Errors;
using ECommerce.Business.DTOs.Pagination;
using ECommerce.Business.Interfaces;
using ECommerce.Core.Specifications.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Tags("Categories Management")]
    public class CategoriesController(ICategoryService categories) : ControllerBase
    {
        private readonly ICategoryService _categories = categories;


        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Get all categories with paging and search support.")]
        [ProducesResponseType(typeof(PagedResponseDto<AdminCategoryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAllCategoriesAdmin([FromQuery] AdminCategorySpecParams specParams)
        {
            var result = await _categories.GetAllCategoriesAdminAsync(specParams);
            return Ok(result);
        }

        [HttpGet("admin/{categoryId:int}")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Get category details.")]
        [ProducesResponseType(typeof(AdminCategoryDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCategoryDetailsAdmin([FromRoute] int categoryId)
        {
            var category = await _categories.GetCategoryAdminAsync(categoryId);
            return Ok(category);
        }

        [HttpPost("admin")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Create new category.")]
        [ProducesResponseType(typeof(AdminCategoryDetailsDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateCategoryAdmin([FromBody] AdminCreateCategoryDto dto)
        {
            var categoryCreated = await _categories.CreateCategoryAdminAsync(dto);
            return CreatedAtAction(nameof(GetCategoryDetailsAdmin), new { categoryCreated.Id }, categoryCreated);
        }


        [HttpPut("admin/{categoryId:int}")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Update existing cateogry with its children.")]
        [ProducesResponseType(typeof(AdminCategoryDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCategoryAdmin([FromRoute] int categoryId, [FromBody] AdminUpdateCategoryDto dto)
        {
            var categoryUpdated = await _categories.UpdateCategoryAdminAsync(categoryId, dto);
            return Ok(categoryUpdated);
        }

        [HttpDelete("admin/{categoryId:int}")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Deletes a category if no product nor subcategories are referencing it.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteCategoryAdmin([FromRoute] int categoryId)
        {
            await _categories.DeleteCategoryAdminAsync(categoryId);
            return NoContent();
        }

        [HttpGet]
        [EndpointSummary("Get all categories nested tree hierarchy.")]
        [ProducesResponseType(typeof(List<CategoryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllCateories()
        {
            var categoriesTree = await _categories.GetAllCategories();
            return Ok(categoriesTree);
        }
    }
}
