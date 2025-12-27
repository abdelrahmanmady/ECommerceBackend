using AutoMapper;
using AutoMapper.QueryableExtensions;
using ECommerce.Business.DTOs.Pagination;
using ECommerce.Business.DTOs.Products;
using ECommerce.Business.DTOs.Products.Requests;
using ECommerce.Business.DTOs.Products.Responses;
using ECommerce.Business.Interfaces;
using ECommerce.Core.Entities;
using ECommerce.Core.Exceptions;
using ECommerce.Core.Specifications.Products;
using ECommerce.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ECommerce.Business.Services
{
    public class ProductService(AppDbContext context, IMapper mapper, ILogger<ProductService> logger) : IProductService
    {
        private readonly AppDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<ProductService> _logger = logger;


        public async Task<PagedResponse<AdminProductSummaryDto>> GetAllProductsAdminAsync(AdminProductSpecParams specParams)
        {
            var query = _context.Products.IgnoreQueryFilters().AsNoTracking().AsQueryable();

            //Filter
            query = specParams.Status switch
            {
                "active" => query.Where(p => !p.IsDeleted),
                "archived" => query.Where(p => p.IsDeleted),
                _ => query
            };

            //Search
            if (!string.IsNullOrEmpty(specParams.Search))
            {
                var term = specParams.Search.ToLower();
                bool isNumeric = int.TryParse(term, out int searchId);


                query = query.Where(p =>
                    (isNumeric && p.Id == searchId) ||
                    p.Name.ToLower().Contains(term) ||
                    (p.Description != null && p.Description.ToLower().Contains(term)) ||
                    p.Category.Name.ToLower().Contains(term)
                );
            }

            //Default order
            query = query.OrderBy(p => p.Id);

            //Pagination

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((specParams.PageIndex - 1) * specParams.PageSize)
                .Take(specParams.PageSize)
                .ProjectTo<AdminProductSummaryDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new PagedResponse<AdminProductSummaryDto>
            {
                PageIndex = specParams.PageIndex,
                PageSize = specParams.PageSize,
                TotalCount = totalCount,
                Items = items
            };
        }

        public async Task<AdminProductDetailsResponse> GetProductDetailsAdminAsync(int productId)
        {
            var product = await _context.Products
                .AsNoTracking()
                .IgnoreQueryFilters()
                .Where(p => p.Id == productId)
                .ProjectTo<AdminProductDetailsResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException("Product does not exist.");
            return product;
        }

        public async Task<AdminProductDetailsResponse> CreateProductAdminAsync(CreateProductRequest createProductRequest)
        {
            //Validate Category
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == createProductRequest.CategoryId);
            if (!categoryExists)
                throw new NotFoundException("Category does not exist.");
            //Validate Category is Terminal (Leaf Node)
            // If any other category has this ID as its Parent, then this is NOT a leaf.
            var isParentCategory = await _context.Categories.AnyAsync(c => c.ParentId == createProductRequest.CategoryId);
            if (isParentCategory)
            {
                throw new BadRequestException("You cannot add products to a Parent Category. Please select a sub-category.");
            }
            //Validate Brand
            var brandExists = await _context.Brands.AnyAsync(b => b.Id == createProductRequest.BrandId);
            if (!brandExists)
                throw new NotFoundException("Brand does not exist");

            var productToAdd = _mapper.Map<Product>(createProductRequest);

            _context.Products.Add(productToAdd);

            await _context.SaveChangesAsync();

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Product added with id = {id}.", productToAdd.Id);

            return _mapper.Map<AdminProductDetailsResponse>(productToAdd);
        }

        public async Task<AdminProductDetailsResponse> UpdateProductAdminAsync(int productId, UpdateProductRequest updateProductRequest)
        {
            var productToUpdate = await _context.Products.FindAsync(productId)
                ?? throw new NotFoundException("Product does not exist.");

            //Validate Category
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == updateProductRequest.CategoryId);
            if (!categoryExists)
                throw new NotFoundException("Category does not exist.");
            //Validate Category is Terminal (Leaf Node)
            // If any other category has this ID as its Parent, then this is NOT a leaf.
            var isParentCategory = await _context.Categories.AnyAsync(c => c.ParentId == updateProductRequest.CategoryId);
            if (isParentCategory)
            {
                throw new BadRequestException("You cannot add products to a Parent Category. Please select a sub-category.");
            }

            //Validate Brand
            var brandExists = await _context.Brands.AnyAsync(b => b.Id == updateProductRequest.BrandId);
            if (!brandExists)
                throw new NotFoundException("Brand does not exist");

            //Update Product
            _mapper.Map(updateProductRequest, productToUpdate);

            if (productToUpdate.Images.Count == 0 || !productToUpdate.Images.Any(pi => pi.IsMain))
                throw new BadRequestException("Product must have a main image before saving.");
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConflictException("Product changed while upating, Please try again.");
            }

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Product updated with id = {productId}.", productId);

            return _mapper.Map<AdminProductDetailsResponse>(productToUpdate);
        }

        public async Task DeleteProductAdminAsync(int productId)
        {
            var productToDelete = await _context.Products.FindAsync(productId)
                ?? throw new NotFoundException("Product does not exist.");
            productToDelete.IsDeleted = true;
            await _context.SaveChangesAsync();

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Product deleted with id = {productId}.", productId);
        }

        public async Task<PagedResponse<ProductSummaryDto>> GetAllProductsAsync(ProductSpecParams specParams)
        {
            var query = _context.Products
                .AsNoTracking()
                .AsQueryable();

            //Filter
            //show in stock products only.
            query = query.Where(p => p.StockQuantity > 0);

            //Filter with Brands
            if (specParams.BrandsIdsList.Count > 0)
            {
                query = query.Where(p => specParams.BrandsIdsList.Contains(p.BrandId));
            }

            //Filter with Category
            if (specParams.CategoryId.HasValue)
            {
                var filterCategory = await _context.Categories
                    .AsNoTracking()
                    .Where(c => c.Id == specParams.CategoryId)
                    .FirstOrDefaultAsync()
                    ?? throw new NotFoundException("Category does not exist.");

                query = query.Where(p => p.Category.HierarchyPath.StartsWith(filterCategory.HierarchyPath));
            }

            //Filter with MinPrice
            if (specParams.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= specParams.MinPrice);
            }

            //Filter with MaxPrice
            if (specParams.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= specParams.MaxPrice);
            }

            //Search
            if (!string.IsNullOrEmpty(specParams.Search))
            {
                query = query.Where(p => p.Name.Contains(specParams.Search));
            }

            //Sort
            query = specParams.Sort switch
            {
                "featured" => query.OrderByDescending(p => p.IsFeatured),
                "priceAsc" => query.OrderBy(p => p.Price),
                "priceDesc" => query.OrderByDescending(p => p.Price),
                "newest" => query.OrderByDescending(p => p.Created),
                _ => query.OrderBy(p => p.Name)
            };

            //Paginate

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((specParams.PageIndex - 1) * specParams.PageSize)
                .Take(specParams.PageSize)
                .ProjectTo<ProductSummaryDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            var categories = await _context.Categories
                .AsNoTracking()
                .Select(c => new { c.Id, c.Name, c.ParentId })
                .ToDictionaryAsync(c => c.Id);

            foreach (var item in items)
            {
                int? currentCategoryId = item.CategoryId;

                while (currentCategoryId.HasValue)
                {
                    var categoryExists = categories.TryGetValue(currentCategoryId.Value, out var category);

                    if (categoryExists && category != null)
                    {
                        item.CategoryBreadcrumbLinks.Add(new BreadcrumbLink
                        {
                            Id = category.Id,
                            Name = category.Name,
                        });
                    }

                    currentCategoryId = category?.ParentId;
                }
                item.CategoryBreadcrumbLinks.Reverse();
            }

            return new PagedResponse<ProductSummaryDto>
            {
                PageIndex = specParams.PageIndex,
                PageSize = specParams.PageSize,
                TotalCount = totalCount,
                Items = items
            };
        }

        public async Task<ProductDetailsResponse> GetProductDetailsAsync(int productId)
        {
            var product = await _context.Products
                .AsNoTracking()
                .Where(p => p.Id == productId)
                .ProjectTo<ProductDetailsResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException("Product does not exist.");
            return product;
        }


    }
}

