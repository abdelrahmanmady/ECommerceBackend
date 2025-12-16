using ECommerce.Business.DTOs.Errors;
using ECommerce.Business.DTOs.Pagination;
using ECommerce.Business.DTOs.Products.Admin;
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
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(AdminProductDetailsDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateProductAdmin([FromForm] AdminCreateProductDto dto)
        {
            var createdProduct = await _products.CreateProductAdminAsync(dto);
            return CreatedAtAction(nameof(GetProductDetailsAdmin), new { id = createdProduct.Id }, createdProduct);
        }

        //[HttpPut("{id:int}")]
        //[Authorize(Roles = "Admin")]
        //[EndpointSummary("Update product")]
        //[EndpointDescription("Updates product details. Concurrency safe.")]
        //[ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        //[ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        //public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateProductDto dto)
        //{
        //    var updatedProduct = await _products.UpdateAsync(id, dto);
        //    return Ok(updatedProduct);
        //}

        //[HttpDelete("{id:int}")]
        //[Authorize(Roles = "Admin")]
        //[EndpointSummary("Delete product")]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        //[ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        //public async Task<IActionResult> Delete([FromRoute] int id)
        //{
        //    await _products.DeleteAsync(id);
        //    return NoContent();
        //}


        //[HttpGet]
        //[EndpointSummary("Get all products")]
        //[EndpointDescription("Retrieves a list of all products.")]
        //[ProducesResponseType(typeof(PagedResponseDto<ProductDto>), StatusCodes.Status200OK)]
        //public async Task<IActionResult> GetAll([FromQuery] ProductSpecParams specParams) => Ok(await _products.GetProductsForCustomerAsync(specParams));
    }
}
