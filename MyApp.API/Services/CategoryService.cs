using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MyApp.API.Data;
using MyApp.API.DTOs.Categories;
using MyApp.API.Entities;
using MyApp.API.Interfaces;

namespace MyApp.API.Services
{
    public class CategoryService(AppDbContext context, IMapper mapper) : ICategoryService
    {
        private readonly AppDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            return await _context.Categories
                .ProjectTo<CategoryDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            return await _context.Categories.Where(x => x.Id == id)
                .ProjectTo<CategoryDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
        {
            var categoryToAdd = _mapper.Map<Category>(dto);
            _context.Categories.Add(categoryToAdd);
            await _context.SaveChangesAsync();
            return _mapper.Map<CategoryDto>(categoryToAdd);
        }

        public async Task<CategoryDto?> UpdateAsync(int id, UpdateCategoryDto dto)
        {
            var categoryToUpdate = await _context.Categories.FindAsync(id);
            if (categoryToUpdate is null)
                return null;
            _mapper.Map(dto, categoryToUpdate);
            await _context.SaveChangesAsync();
            return _mapper.Map<CategoryDto>(categoryToUpdate);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var categoryToDelete = await _context.Categories.FindAsync(id);
            if (categoryToDelete is null)
                return false;
            _context.Categories.Remove(categoryToDelete);
            await _context.SaveChangesAsync();
            return true;

        }
    }
}
