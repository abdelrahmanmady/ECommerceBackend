using ECommerce.Business.DTOs.Products.Admin;
using Microsoft.AspNetCore.Http;

namespace ECommerce.Business.Interfaces
{
    public interface IProductImageService
    {
        Task<AdminProductDetailsDto> AddImagesAsync(int productId, List<IFormFile> files);
        Task SetMainImageAsync(int productIdFromRotue, int imageId);
        Task DeleteImageAsync(int productIdFromRoute, int imageId);
    }
}
