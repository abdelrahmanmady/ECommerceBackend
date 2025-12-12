using ECommerce.Business.Interfaces;
using ECommerce.Core.Exceptions;
using ECommerce.Core.Settings;
using Microsoft.Extensions.Options;

namespace ECommerce.API.Infrastructure
{
    public class LocalFileStorageService(IWebHostEnvironment webHostEnvironment, IOptions<FileUploadSettings> options) : IFileStorageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;
        private readonly FileUploadSettings _settings = options.Value;

        public async Task<string> SaveFileAsync(IFormFile file, string folderName)
        {
            //Validate file
            if (file == null || file.Length == 0)
                throw new BadRequestException("No file uploaded.");
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_settings.AllowedExtensions.Contains(extension))
                throw new BadRequestException($"Invalid image format. Allowed: {string.Join(", ", _settings.AllowedExtensions)}"); long maxBytes = _settings.MaxFileSizeInMB * 1024 * 1024;
            if (file.Length > maxBytes) // 5MB limit
                throw new BadRequestException($"File size cannot exceed {_settings.MaxFileSizeInMB}MB.");

            //create path of save in wwwroot wwwroot/images/folderName
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", folderName);
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            //create unique name for file with the same extension as the input file.
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/images/{folderName}/{fileName}";
        }

        public async Task DeleteFileAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !filePath.StartsWith("/images/"))
            {
                return;
            }
            var fullpath = Path.Combine(_webHostEnvironment.WebRootPath, filePath.TrimStart('/'));
            if (File.Exists(fullpath))
                File.Delete(fullpath);

        }
    }
}
