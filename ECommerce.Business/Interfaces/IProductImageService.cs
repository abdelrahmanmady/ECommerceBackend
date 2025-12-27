using ECommerce.Business.DTOs.ProductImages.Responses;
using Microsoft.AspNetCore.Http;

namespace ECommerce.Business.Interfaces
{
    public interface IProductImageService
    {
        Task<IEnumerable<ProductImageDto>> AddImagesAsync(int productId, List<IFormFile> files);
        Task SetMainImageAsync(int productIdFromRotue, int imageId);
        Task DeleteImageAsync(int productIdFromRoute, int imageId);
    }
}
