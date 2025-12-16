using ECommerce.Business.DTOs.ProductImages;
using Microsoft.AspNetCore.Http;

namespace ECommerce.Business.Interfaces
{
    public interface IProductImageService
    {
        Task<IEnumerable<ProductImageDto>> GetAllAsync(int productId);
        Task AddImagesAsync(int productId, List<IFormFile> images);
        Task SetMainImageAsync(int productIdFromRotue, int imageId);
        Task DeleteImageAsync(int productIdFromRoute, int imageId);
    }
}
