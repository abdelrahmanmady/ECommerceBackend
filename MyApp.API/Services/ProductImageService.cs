using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MyApp.API.Data;
using MyApp.API.DTOs.ProductImages;
using MyApp.API.Entities;
using MyApp.API.Interfaces;

namespace MyApp.API.Services
{
    public class ProductImageService(AppDbContext context, IMapper mapper) : IProductImageService
    {
        private readonly AppDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<ProductImageDto>> GetAllAsync(int productId)
        {
            var productExists = await _context.Products.AnyAsync(p => p.Id == productId);
            if (!productExists)
                throw new Exception("Invalid ProductId");
            return await _context.ProductImages
                .Where(pi => pi.ProductId == productId)
                .ProjectTo<ProductImageDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<ProductImageDto> AddImageAsync(int productId, AddProductImageDto dto)
        {
            var productExists = await _context.Products.AnyAsync(p => p.Id == productId);
            if (!productExists)
                throw new Exception("Invalid ProductId");
            var productImageToAdd = _mapper.Map<ProductImage>(dto);
            productImageToAdd.ProductId = productId;
            _context.ProductImages.Add(productImageToAdd);
            await _context.SaveChangesAsync();
            return _mapper.Map<ProductImageDto>(productImageToAdd);

        }

        public async Task SetMainImageAsync(int productIdFromRoute, int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId)
                ?? throw new Exception("Invalid ImageId");

            if (image.ProductId != productIdFromRoute)
                throw new Exception("Image does not belong to this product.");

            var product = await _context.Products.FindAsync(image.ProductId)
                ?? throw new Exception("Product not found.");

            var currentMainImage = await _context.ProductImages
                .FirstOrDefaultAsync(pi => pi.ProductId == product.Id && pi.IsMain);

            if (currentMainImage?.Id == imageId)
                return;

            if (currentMainImage != null)
            {
                currentMainImage.IsMain = false;
                await _context.SaveChangesAsync();
            }
            image.IsMain = true;

            product.ImageUrl = image.ImageUrl;

            await _context.SaveChangesAsync();

        }

        public async Task DeleteImageAsync(int productIdFromRoute, int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId)
                ?? throw new Exception("Invalid ImageId");
            if (image.ProductId != productIdFromRoute)
                throw new Exception("Image does not belong to this product.");

            _context.ProductImages.Remove(image);
            await _context.SaveChangesAsync();
        }
    }
}
