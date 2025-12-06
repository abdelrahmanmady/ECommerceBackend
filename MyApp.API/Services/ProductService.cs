using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MyApp.API.Data;
using MyApp.API.DTOs.Products;
using MyApp.API.Entities;
using MyApp.API.Exceptions;
using MyApp.API.Interfaces;

namespace MyApp.API.Services
{
    public class ProductService(AppDbContext context, IMapper mapper) : IProductService
    {
        private readonly AppDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            return await _context.Products.ProjectTo<ProductDto>(_mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<ProductDto> GetByIdAsync(int id)
        {
            return await _context.Products.Where(p => p.Id == id)
                .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException("Product does not exist.");
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            //Validate Category
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
            if (!categoryExists)
                throw new NotFoundException("Category does not exist.");

            //Validate Brand
            var brandExists = await _context.Brands.AnyAsync(b => b.Id == dto.BrandId);
            if (!brandExists)
                throw new NotFoundException("Brand does not exist");

            var productToAdd = _mapper.Map<Product>(dto);
            _context.Products.Add(productToAdd);
            await _context.SaveChangesAsync();
            return _mapper.Map<ProductDto>(productToAdd);
        }

        public async Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto)
        {

            var productToUpdate = await _context.Products.FindAsync(id)
                ?? throw new NotFoundException("Product does not exist.");

            //Validate Category
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
            if (!categoryExists)
                throw new NotFoundException("Category does not exist.");

            //Validate Brand
            var brandExists = await _context.Brands.AnyAsync(b => b.Id == dto.BrandId);
            if (!brandExists)
                throw new NotFoundException("Brand does not exist");

            _mapper.Map(dto, productToUpdate);
            await _context.SaveChangesAsync();
            return _mapper.Map<ProductDto>(productToUpdate);
        }

        public async Task DeleteAsync(int id)
        {
            var productToDelete = await _context.Products.FindAsync(id)
                ?? throw new NotFoundException("Product does not exist.");
            _context.Products.Remove(productToDelete);
            await _context.SaveChangesAsync();
        }


    }
}
