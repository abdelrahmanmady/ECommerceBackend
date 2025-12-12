using AutoMapper;
using AutoMapper.QueryableExtensions;
using ECommerce.Business.DTOs.ProductImages;
using ECommerce.Business.Interfaces;
using ECommerce.Core.Entities;
using ECommerce.Core.Exceptions;
using ECommerce.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ECommerce.Business.Services
{
    public class ProductImageService(
        AppDbContext context,
        IMapper mapper,
        ILogger<ProductImageService> logger,
        IFileStorageService fileStorageService) : IProductImageService
    {
        private readonly AppDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<ProductImageService> _logger = logger;
        private readonly IFileStorageService _fileStorageService = fileStorageService;

        public async Task<IEnumerable<ProductImageDto>> GetAllAsync(int productId)
        {
            var productExists = await _context.Products.AnyAsync(p => p.Id == productId);
            if (!productExists)
                throw new NotFoundException("Product does not exist.");
            return await _context.ProductImages
                .AsNoTracking()
                .Where(pi => pi.ProductId == productId)
                .ProjectTo<ProductImageDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductImageDto>> AddImagesAsync(int productId, AddProductImageDto dto)
        {
            var productExists = await _context.Products.AnyAsync(p => p.Id == productId);
            if (!productExists)
                throw new NotFoundException("Product does not exist.");


            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Starting upload of {Count} images for Product {ProductId}",
                    dto.Images.Count,
                    productId);
            }

            var uploadedImages = new List<ProductImage>();
            var uploadedFileNames = new List<string>();

            foreach (var image in dto.Images)
            {

                var relativePath = await _fileStorageService.SaveFileAsync(image, "products");
                var fileName = Path.GetFileName(relativePath);

                var productImageToAdd = new ProductImage
                {
                    ProductId = productId,
                    ImageUrl = relativePath
                };

                if (!await _context.ProductImages.AnyAsync(p => p.ProductId == productId))
                {
                    productImageToAdd.IsMain = true;
                }

                uploadedImages.Add(productImageToAdd);
                uploadedFileNames.Add(fileName);

            }

            _context.ProductImages.AddRange(uploadedImages);
            await _context.SaveChangesAsync();

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Successfully added {Count} images to Product {ProductId}. Generated Files: {FileNames}",
                    uploadedImages.Count,
                    productId,
                    string.Join(", ", uploadedFileNames));
            }
            return _mapper.Map<IEnumerable<ProductImageDto>>(uploadedImages);

        }

        public async Task SetMainImageAsync(int productIdFromRoute, int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId)
                ?? throw new NotFoundException("Image does not exist.");

            if (image.ProductId != productIdFromRoute)
                throw new BadRequestException("Image does not belong to this product.");

            var product = await _context.Products.FindAsync(image.ProductId)
                ?? throw new NotFoundException("Product does not exist.");

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

            await _context.SaveChangesAsync();
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Image with id = {imageId} for product = {productId} is set as main.", image.Id, image.ProductId);

        }

        public async Task DeleteImageAsync(int productIdFromRoute, int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId)
                ?? throw new NotFoundException("Image does not exist.");

            if (image.ProductId != productIdFromRoute)
                throw new BadRequestException("Image does not belong to this product.");

            var product = await _context.Products.FindAsync(image.ProductId)
                ?? throw new NotFoundException("Product does not exist.");

            //Throw ConflictException
            if (image.IsMain)
                throw new ConflictException("Cannot delete the main image. Set another image as main first.");

            await _fileStorageService.DeleteFileAsync(image.ImageUrl);

            _context.ProductImages.Remove(image);
            await _fileStorageService.DeleteFileAsync(image.ImageUrl);
            await _context.SaveChangesAsync();

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Image deleted with id = {imageId} for product = {productId}.", image.Id, image.ProductId);
        }
    }
}
