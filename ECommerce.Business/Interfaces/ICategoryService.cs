using ECommerce.Business.DTOs.Categories.Responses;
using ECommerce.Business.DTOs.Pagination;
using ECommerce.Core.Specifications.Categories;

namespace ECommerce.Business.Interfaces
{
    public interface ICategoryService
    {
        Task<PagedResponse<AdminCategorySummaryDto>> GetAllCategoriesAdminAsync(AdminCategorySpecParams specParams);
        Task<AdminCategoryDetailsResponse> GetCategoryAdminAsync(int categoryId);
        //Task<CategoryDetailsResponse> CreateCategoryAdminAsync(CreateCategoryRequest createCategoryRequest);
        //Task<CategoryDetailsResponse> UpdateCategoryAdminAsync(int categoryId, UpdateCategoryRequest updateCategoryRequest);
        //Task DeleteCategoryAdminAsync(int categoryId);
        Task<List<CategorySummaryDto>> GetAllCategoriesAsync();
    }
}
