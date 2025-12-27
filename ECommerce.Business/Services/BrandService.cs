using AutoMapper;
using AutoMapper.QueryableExtensions;
using ECommerce.Business.DTOs.Brands.Requests;
using ECommerce.Business.DTOs.Brands.Responses;
using ECommerce.Business.DTOs.Pagination;
using ECommerce.Business.Interfaces;
using ECommerce.Core.Entities;
using ECommerce.Core.Exceptions;
using ECommerce.Core.Specifications.Brands;
using ECommerce.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ECommerce.Business.Services
{
    public class BrandService(AppDbContext context, IMapper mapper, ILogger<BrandService> logger) : IBrandService
    {
        private readonly AppDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<BrandService> _logger = logger;

        public async Task<PagedResponse<AdminBrandSummaryDto>> GetAllBrandsAdminAsync(AdminBrandSpecParams specParams)
        {
            var query = _context.Brands.AsNoTracking().AsQueryable();

            //Default Sort
            query = query.OrderBy(b => b.Id);

            //Search
            if (!string.IsNullOrEmpty(specParams.Search))
            {
                query = query.Where(b => (b.Name.Contains(specParams.Search))
                || (b.Description != null && b.Description.Contains(specParams.Search)));
            }

            //Pagination
            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((specParams.PageIndex - 1) * specParams.PageSize)
                .Take(specParams.PageSize)
                .ProjectTo<AdminBrandSummaryDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new PagedResponse<AdminBrandSummaryDto>
            {
                PageIndex = specParams.PageIndex,
                PageSize = specParams.PageSize,
                TotalCount = totalCount,
                Items = items
            };

        }

        public async Task<BrandDetailsResponse> GetBrandDetailsAdminAsync(int brandId)
        {
            var brand = await _context.Brands
                .AsNoTracking()
                .Where(b => b.Id == brandId)
                .ProjectTo<BrandDetailsResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException("Brand does not exist.");
            return brand;
        }

        public async Task<BrandDetailsResponse> CreateBrandAdminAsync(CreateBrandRequest createBrandRequest)
        {
            var brandToCreate = _mapper.Map<Brand>(createBrandRequest);
            _context.Brands.Add(brandToCreate);
            await _context.SaveChangesAsync();

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Brand added with id = {id}.", brandToCreate.Id);

            return _mapper.Map<BrandDetailsResponse>(brandToCreate);
        }

        public async Task<BrandDetailsResponse> UpdateBrandAdminAsync(int brandId, UpdateBrandRequest updateBrandRequest)
        {
            var brandToUpdate = await _context.Brands.FindAsync(brandId)
                ?? throw new NotFoundException("Brand does not exist");

            _mapper.Map(updateBrandRequest, brandToUpdate);
            await _context.SaveChangesAsync();

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Brand updated with id = {id}.", brandId);

            return _mapper.Map<BrandDetailsResponse>(brandToUpdate);
        }

        public async Task DeleteBrandAdminAsync(int brandId)
        {
            var brandToDelete = await _context.Brands.FindAsync(brandId)
                ?? throw new NotFoundException("Brand does not exist.");

            var hasProducts = await _context.Products.AnyAsync(p => p.BrandId == brandId);
            if (hasProducts)
                throw new ConflictException("Cannot delete a category having products.");

            _context.Brands.Remove(brandToDelete);
            await _context.SaveChangesAsync();

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Brand deleted with id = {id}.", brandId);

        }

        public async Task<IEnumerable<BrandSummaryDto>> GetAllBrandsAsync()
        {
            var brands = await _context.Brands
                .AsNoTracking()
                .ProjectTo<BrandSummaryDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
            return brands;
        }
    }
}
