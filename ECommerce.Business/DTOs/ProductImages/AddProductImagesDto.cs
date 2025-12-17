using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Business.DTOs.ProductImages
{
    public class AddProductImagesDto
    {
        [MinLength(1, ErrorMessage = ("At least one image must be added."))]
        public List<IFormFile> Files { get; set; } = [];
    }
}
