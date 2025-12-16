using ECommerce.Business.DTOs.Pagination;
using ECommerce.Business.DTOs.Products.Admin;
using ECommerce.Core.Specifications.Products;

namespace ECommerce.Business.Interfaces
{
    public interface IProductService
    {

        Task<PagedResponseDto<AdminProductDto>> GetAllProductsAdminAsync(AdminProductSpecParams specParams);
        Task<AdminProductDetailsDto> GetProductDetailsAdminAsync(int productId);
        Task<AdminProductDetailsDto> CreateProductAdminAsync(AdminCreateProductDto dto);
        //Task<AdminProductDetailsDto> UpdateProductAdminAsync(int productId, AdminUpdateProductDto dto);
        //Task DeleteProductAdminAsync(int productId);

        //Task<PagedResponseDto<ProductDto>> GetAllProductsAsync(ProductSpecParams specParams);
        //Task<ProductDetailsDto> GetProductDetailsAsync();



    }
}
