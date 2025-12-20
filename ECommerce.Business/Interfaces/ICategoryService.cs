using ECommerce.Business.DTOs.Categories.Admin;
using ECommerce.Business.DTOs.Categories.Store;
using ECommerce.Business.DTOs.Pagination;
using ECommerce.Core.Specifications.Categories;

namespace ECommerce.Business.Interfaces
{
    public interface ICategoryService
    {
        Task<PagedResponseDto<AdminCategoryDto>> GetAllCategoriesAdminAsync(AdminCategorySpecParams specParams);
        Task<AdminCategoryDetailsDto> GetCategoryAdminAsync(int categoryId);
        Task<AdminCategoryDetailsDto> CreateCategoryAdminAsync(AdminCreateCategoryDto dto);
        Task<AdminCategoryDetailsDto> UpdateCategoryAdminAsync(int categoryId, AdminUpdateCategoryDto dto);
        Task DeleteCategoryAdminAsync(int categoryId);
        Task<List<CategoryDto>> GetAllCategories();
    }
}
