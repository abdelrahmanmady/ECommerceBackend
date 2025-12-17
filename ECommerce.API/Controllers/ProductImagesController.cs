using ECommerce.Business.DTOs.Errors;
using ECommerce.Business.DTOs.ProductImages;
using ECommerce.Business.DTOs.Products.Admin;
using ECommerce.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/Products/{productId:int}/images")]
    [ApiController]
    [Tags("Products Management")]
    public class ProductImagesController(IProductImageService productImages) : ControllerBase
    {
        private readonly IProductImageService _productImages = productImages;


        [HttpPost]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Upload product images.")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(AdminProductDetailsDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Add([FromRoute] int productId, [FromForm] AddProductImagesDto dto)
        {
            var product = await _productImages.AddImagesAsync(productId, dto.Files);
            return StatusCode(StatusCodes.Status201Created, product);
        }




        [HttpPut("{imageId:int}/setmain")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Set main image")]
        [EndpointDescription("Updates the product's main thumbnail image.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetMainImage([FromRoute] int productId, [FromRoute] int imageId)
        {
            await _productImages.SetMainImageAsync(productId, imageId);
            return NoContent();
        }

        [HttpDelete("{imageId:int}")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Delete image")]
        [EndpointDescription("Removes an image. Cannot delete the Main image.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponseDto), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteImage([FromRoute] int productId, [FromRoute] int imageId)
        {
            await _productImages.DeleteImageAsync(productId, imageId);
            return NoContent();
        }



    }
}
