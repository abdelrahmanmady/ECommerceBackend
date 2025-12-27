using ECommerce.Business.DTOs.Pagination;
using ECommerce.Business.DTOs.Products.Requests;
using ECommerce.Business.DTOs.Products.Responses;
using ECommerce.Core.Specifications.Products;

namespace ECommerce.Business.Interfaces
{
    public interface IProductService
    {

        Task<PagedResponse<AdminProductSummaryDto>> GetAllProductsAdminAsync(AdminProductSpecParams specParams);
        Task<AdminProductDetailsResponse> GetProductDetailsAdminAsync(int productId);
        Task<AdminProductDetailsResponse> CreateProductAdminAsync(CreateProductRequest createProductRequest);
        Task<AdminProductDetailsResponse> UpdateProductAdminAsync(int productId, UpdateProductRequest updateProductRequest);
        Task DeleteProductAdminAsync(int productId);

        Task<PagedResponse<ProductSummaryDto>> GetAllProductsAsync(ProductSpecParams specParams);
        Task<ProductDetailsResponse> GetProductDetailsAsync(int productId);
    }
}
