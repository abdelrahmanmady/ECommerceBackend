using Microsoft.AspNetCore.Http;

namespace ECommerce.Business.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file, string folderName);
        Task DeleteFileAsync(string filePath);
    }
}
