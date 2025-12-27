using ECommerce.Business.DTOs.Brands.Requests;
using ECommerce.Business.DTOs.Brands.Responses;
using ECommerce.Business.DTOs.Pagination;
using ECommerce.Core.Specifications.Brands;

namespace ECommerce.Business.Interfaces
{
    public interface IBrandService
    {
        Task<PagedResponse<AdminBrandSummaryDto>> GetAllBrandsAdminAsync(AdminBrandSpecParams specParams);
        Task<BrandDetailsResponse> GetBrandDetailsAdminAsync(int brandId);
        Task<BrandDetailsResponse> CreateBrandAdminAsync(CreateBrandRequest createBrandRequest);
        Task<BrandDetailsResponse> UpdateBrandAdminAsync(int brandId, UpdateBrandRequest updateBrandRequest);
        Task DeleteBrandAdminAsync(int brandId);
        Task<IEnumerable<BrandSummaryDto>> GetAllBrandsAsync();
    }
}
