using Microsoft.AspNetCore.Http;

namespace ECommerce.Business.DTOs.ProductImages
{
    public class AddProductImageDto
    {
        public List<IFormFile> Images { get; set; } = [];
    }
}
