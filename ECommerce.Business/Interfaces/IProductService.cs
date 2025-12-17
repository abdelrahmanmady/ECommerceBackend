using ECommerce.Business.DTOs.Pagination;
using ECommerce.Business.DTOs.Products.Admin;
using ECommerce.Business.DTOs.Products.Store;
using ECommerce.Core.Specifications.Products;

namespace ECommerce.Business.Interfaces
{
    public interface IProductService
    {

        Task<PagedResponseDto<AdminProductDto>> GetAllProductsAdminAsync(AdminProductSpecParams specParams);
        Task<AdminProductDetailsDto> GetProductDetailsAdminAsync(int productId);
        Task<int> CreateProductAdminAsync(AdminCreateProductDto dto);
        Task<AdminProductDetailsDto> UpdateProductAdminAsync(int productId, AdminUpdateProductDto dto);
        Task DeleteProductAdminAsync(int productId);
        Task<PagedResponseDto<ProductDto>> GetAllProductsAsync(ProductSpecParams specParams);
        Task<ProductDetailsDto> GetProductDetailsAsync(int productId);
    }
}
