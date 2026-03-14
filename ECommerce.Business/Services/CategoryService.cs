using AutoMapper;
using AutoMapper.QueryableExtensions;
using ECommerce.Business.DTOs.Breadcrumb;
using ECommerce.Business.DTOs.Categories.Responses;
using ECommerce.Business.DTOs.Pagination;
using ECommerce.Business.Interfaces;
using ECommerce.Core.Exceptions;
using ECommerce.Core.Specifications.Categories;
using ECommerce.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ECommerce.Business.Services
{
    public class CategoryService(AppDbContext context, IMapper mapper, ILogger<CategoryService> logger) : ICategoryService
    {
        private readonly AppDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<CategoryService> _logger = logger;

        public async Task<PagedResponse<AdminCategorySummaryDto>> GetAllCategoriesAdminAsync(AdminCategorySpecParams specParams)
        {
            var query = _context.Categories
                .AsNoTracking()
                .OrderBy(c => c.Created)
                .AsQueryable();

            //Filter
            if (!string.IsNullOrEmpty(specParams.Type))
            {
                if (specParams.Type == "leaf")
                {
                    query = query.Where(c => c.Subcategories.Count == 0);
                }
                else if (specParams.Type == "node")
                {
                    query = query.Where(c => c.Subcategories.Count > 0);
                }
            }

            //Search
            if (!string.IsNullOrEmpty(specParams.Search))
            {
                query = query.Where(c => c.Name.Contains(specParams.Search)
                || (c.Description != null && c.Description.Contains(specParams.Search))
                || (c.HierarchyPath != null && c.HierarchyPath.Contains($"/{specParams.Search}")));
            }

            //Pagination
            var totalCount = await query.CountAsync();
            var items = await query
               .Skip((specParams.PageIndex - 1) * specParams.PageSize)
               .Take(specParams.PageSize)
               .ProjectTo<AdminCategorySummaryDto>(_mapper.ConfigurationProvider)
               .ToListAsync();

            return new PagedResponse<AdminCategorySummaryDto>
            {
                PageIndex = specParams.PageIndex,
                PageSize = specParams.PageSize,
                TotalCount = totalCount,
                Items = items
            };

        }

        public async Task<AdminCategoryDetailsResponse> GetCategoryAdminAsync(int categoryId)
        {
            var categoryDto = await _context.Categories
                .AsNoTracking()
                .Where(c => c.Id == categoryId)
                .ProjectTo<AdminCategoryDetailsResponse>(_mapper.ConfigurationProvider)
                .AsSplitQuery()
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException("Category does not exist.");

            var categories = await _context.Categories
                .AsNoTracking()
                .Select(c => new { c.Id, c.Name, c.ParentId })
                .ToDictionaryAsync(c => c.Id);

            //Constructing HierarchyBreadcrumb

            int? currentCategoryId = categoryDto.ParentId;

            while (currentCategoryId.HasValue)
            {
                var categoryExists = categories.TryGetValue(currentCategoryId.Value, out var category);
                if (categoryExists && category != null)
                {
                    categoryDto.HierarchyBreadcrumb.Add(new BreadcrumbLink
                    {
                        Id = currentCategoryId.Value,
                        Name = category.Name
                    });
                }
                currentCategoryId = category?.ParentId;
            }
            categoryDto.HierarchyBreadcrumb.Reverse();

            return categoryDto;


        }

        //public async Task<CategoryDetailsResponse> CreateCategoryAdminAsync(CreateCategoryRequest createCategoryRequest)
        //{
        //    string? parentHierarchyPath = null;

        //    //validate parent category
        //    if (createCategoryRequest.ParentId.HasValue)
        //    {
        //        var parentCateory = await _context.Categories
        //            .AsNoTracking()
        //            .Where(c => c.Id == createCategoryRequest.ParentId.Value)
        //            .Select(c => new
        //            {
        //                c.Name,
        //                c.HierarchyPath
        //            })
        //            .FirstOrDefaultAsync()
        //            ?? throw new NotFoundException("Parent Category does not exist.");

        //        parentHierarchyPath = parentCateory.HierarchyPath;
        //    }

        //    var categoryToCreate = _mapper.Map<Category>(createCategoryRequest);
        //    categoryToCreate.HierarchyPath = parentHierarchyPath is null ? categoryToCreate.Name : $"{parentHierarchyPath}\\{categoryToCreate.Name}";

        //    _context.Categories.Add(categoryToCreate);
        //    await _context.SaveChangesAsync();

        //    if (_logger.IsEnabled(LogLevel.Information))
        //        _logger.LogInformation("Category added with id = {id}.", categoryToCreate.Id);

        //    return _mapper.Map<CategoryDetailsResponse>(categoryToCreate);
        //}

        //public async Task<CategoryDetailsResponse> UpdateCategoryAdminAsync(int categoryId, UpdateCategoryRequest updateCategoryRequest)
        //{
        //    var categoryToUpdate = await _context.Categories.FindAsync(categoryId)
        //        ?? throw new NotFoundException("Category does not exist.");

        //    string? parentHierarchyPath = null;
        //    //validate parent category
        //    if (updateCategoryRequest.ParentId.HasValue)
        //    {
        //        var parentCateory = await _context.Categories
        //            .AsNoTracking()
        //            .Where(c => c.Id == updateCategoryRequest.ParentId.Value)
        //            .Select(c => new
        //            {
        //                c.Name,
        //                c.HierarchyPath
        //            })
        //            .FirstOrDefaultAsync()
        //            ?? throw new NotFoundException("Parent Category does not exist.");

        //        parentHierarchyPath = parentCateory.HierarchyPath;
        //    }
        //    _mapper.Map(updateCategoryRequest, categoryToUpdate);
        //    categoryToUpdate.HierarchyPath = parentHierarchyPath is null ? categoryToUpdate.Name : $"{parentHierarchyPath}\\{categoryToUpdate.Name}";
        //    categoryToUpdate.Updated = DateTime.UtcNow;

        //    //updating children of this category
        //    await _context.Categories.Where(c => c.ParentId == categoryToUpdate.Id).ForEachAsync(c =>
        //    {
        //        c.HierarchyPath = $"{categoryToUpdate.HierarchyPath}\\{c.Name}";
        //        c.Updated = DateTime.UtcNow;
        //    });

        //    await _context.SaveChangesAsync();

        //    if (_logger.IsEnabled(LogLevel.Information))
        //        _logger.LogInformation("Category updated with id = {id}.", categoryToUpdate.Id);

        //    return _mapper.Map<CategoryDetailsResponse>(categoryToUpdate);
        //}

        //public async Task DeleteCategoryAdminAsync(int categoryId)
        //{
        //    var categoryToDelete = await _context.Categories.FindAsync(categoryId)
        //        ?? throw new NotFoundException("Category does not exist");

        //    //check if category has subcategories
        //    var hasSubcategories = await _context.Categories.AnyAsync(c => c.ParentId == categoryId);
        //    if (hasSubcategories)
        //        throw new ConflictException("Cannot delete a category having children subcategories.");

        //    //check if category is terminal having products
        //    var hasProducts = await _context.Products.AnyAsync(p => p.CategoryId == categoryId);
        //    if (hasProducts)
        //        throw new ConflictException("Cannot delete a category having products.");

        //    _context.Categories.Remove(categoryToDelete);
        //    await _context.SaveChangesAsync();

        //    if (_logger.IsEnabled(LogLevel.Information))
        //        _logger.LogInformation("Category deleted with id = {id}.", categoryId);
        //}

        public async Task<List<CategorySummaryDto>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .ProjectTo<CategorySummaryDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            var lookup = categories.ToDictionary(c => c.Id);

            var rootCategories = new List<CategorySummaryDto>();

            foreach (var category in categories)
            {
                if (category.ParentId.HasValue && lookup.TryGetValue(category.ParentId.Value, out var parent))
                    parent.Subcategories.Add(category);
                else
                    rootCategories.Add(category);
            }

            return rootCategories;
        }
    }
}
