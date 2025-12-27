using ECommerce.Business.DTOs.Errors;
using ECommerce.Business.DTOs.Pagination;
using ECommerce.Business.DTOs.Products.Requests;
using ECommerce.Business.DTOs.Products.Responses;
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
        [ProducesResponseType(typeof(PagedResponse<AdminProductSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAllProductsAdmin([FromQuery] AdminProductSpecParams specParams)
            => Ok(await _products.GetAllProductsAdminAsync(specParams));

        [HttpGet("admin/{productId:int}")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Get product details")]
        [ProducesResponseType(typeof(AdminProductDetailsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductDetailsAdmin([FromRoute] int productId)
            => Ok(await _products.GetProductDetailsAdminAsync(productId));

        [HttpPost("admin")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Create a new product.")]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateProductAdmin([FromBody] CreateProductRequest createProductRequest)
        {
            var createdProductId = await _products.CreateProductAdminAsync(createProductRequest);
            return StatusCode(StatusCodes.Status201Created, new { createdProductId });
        }

        [HttpPut("admin/{productId:int}")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Updates product details.")]
        [ProducesResponseType(typeof(AdminProductDetailsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Update([FromRoute] int productId, [FromBody] UpdateProductRequest updateProductRequest)
        {
            var updatedProduct = await _products.UpdateProductAdminAsync(productId, updateProductRequest);
            return Ok(updatedProduct);
        }

        [HttpDelete("admin/{productId:int}")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Delete product with its images.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] int productId)
        {
            await _products.DeleteProductAdminAsync(productId);
            return NoContent();
        }

        [HttpGet]
        [EndpointSummary("Get all products.")]
        [ProducesResponseType(typeof(PagedResponse<ProductSummaryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllProducts([FromQuery] ProductSpecParams specParams)
            => Ok(await _products.GetAllProductsAsync(specParams));

        [HttpGet("{productId:int}")]
        [EndpointSummary("Get product details.")]
        [ProducesResponseType(typeof(ProductDetailsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductDetails([FromRoute] int productId)
            => Ok(await _products.GetProductDetailsAsync(productId));
    }
}
