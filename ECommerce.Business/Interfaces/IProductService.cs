using ECommerce.Business.DTOs.Pagination;
using ECommerce.Business.DTOs.Products.Admin;
using ECommerce.Business.DTOs.Products.Management;
using ECommerce.Business.DTOs.Products.Store;
using ECommerce.Core.Specifications;

namespace ECommerce.Business.Interfaces
{
    public interface IProductService
    {
        Task<PagedResponseDto<ProductDto>> GetProductsForCustomerAsync(ProductSpecParams specParams);
        Task<PagedResponseDto<AdminProductDto>> GetProductsForAdminAsync(AdminProductSpecParams specParams);
        Task<ProductDto> GetByIdAsync(int id);
        Task<ProductDto> CreateAsync(CreateProductDto dto);
        Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto);
        Task DeleteAsync(int id);

    }
}
