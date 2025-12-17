using ECommerce.Business.DTOs.Errors;
using ECommerce.Business.DTOs.Pagination;
using ECommerce.Business.DTOs.Products.Admin;
using ECommerce.Business.DTOs.Products.Store;
using ECommerce.Business.Interfaces;
using ECommerce.Core.Specifications.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Tags("Products Management")]
    public class ProductsController(IProductService products) : ControllerBase
    {
        private readonly IProductService _products = products;


        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Get all products for admin dashboard")]
        [ProducesResponseType(typeof(PagedResponseDto<AdminProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAllProductsAdmin([FromQuery] AdminProductSpecParams specParams)
            => Ok(await _products.GetAllProductsAdminAsync(specParams));

        [HttpGet("admin/{productId:int}")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Get product details")]
        [ProducesResponseType(typeof(AdminProductDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductDetailsAdmin([FromRoute] int productId)
            => Ok(await _products.GetProductDetailsAdminAsync(productId));

        [HttpPost("admin")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Create a new product.")]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateProductAdmin([FromBody] AdminCreateProductDto dto)
        {
            var createdProductId = await _products.CreateProductAdminAsync(dto);
            return StatusCode(StatusCodes.Status201Created, new { createdProductId });
        }

        [HttpPut("admin/{productId:int}")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Updates product details.")]
        [ProducesResponseType(typeof(AdminProductDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] AdminUpdateProductDto dto)
        {
            var updatedProduct = await _products.UpdateProductAdminAsync(id, dto);
            return Ok(updatedProduct);
        }

        [HttpDelete("admin/{id:int}")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Delete product with its images.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await _products.DeleteProductAdminAsync(id);
            return NoContent();
        }

        [HttpGet]
        [EndpointSummary("Get all products.")]
        [ProducesResponseType(typeof(PagedResponseDto<ProductDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllProducts([FromQuery] ProductSpecParams specParams)
            => Ok(await _products.GetAllProductsAsync(specParams));

        [HttpGet("{productId:int}")]
        [EndpointSummary("Get product details.")]
        [ProducesResponseType(typeof(ProductDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductDetails([FromRoute] int productId)
            => Ok(await _products.GetProductDetailsAsync(productId));
    }
}
