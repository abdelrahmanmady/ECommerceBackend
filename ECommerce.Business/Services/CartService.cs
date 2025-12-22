using AutoMapper;
using ECommerce.Business.DTOs.ShoppingCart;
using ECommerce.Business.Interfaces;
using ECommerce.Core.Entities;
using ECommerce.Core.Exceptions;
using ECommerce.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ECommerce.Business.Services
{
    public class CartService(
        AppDbContext context,
        IMapper mapper,
        IHttpContextAccessor httpContext,
        ILogger<CartService> logger) : ICartService
    {
        private readonly AppDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly IHttpContextAccessor _httpContext = httpContext;
        private readonly ILogger<CartService> _logger = logger;

        public async Task<ShoppingCartDto> GetAsync()
           => _mapper.Map<ShoppingCartDto>(await GetCartEntityAsync());


        public async Task<ShoppingCartDto> UpdateAsync(UpdateShoppingCartDto dto)
        {
            var cart = await GetCartEntityAsync();

            // 1. Get IDs of products in the incoming DTO
            var incomingProductIds = dto.Items
                .Select(i => i.ProductId)
                .Distinct()
                .ToList();

            // 2. DELETE Logic: Remove items in DB that are NOT in the incoming DTO
            var itemsToRemove = cart.Items
                .Where(i => !incomingProductIds.Contains(i.ProductId))
                .ToList();

            if (itemsToRemove.Count != 0)
            {
                _context.CartItems.RemoveRange(itemsToRemove);
            }


            var productsDict = await _context.Products
                .Include(p => p.Images)
                .Where(p => incomingProductIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            // 3. UPDATE/ADD Logic
            foreach (var itemDto in dto.Items)
            {
                if (!productsDict.TryGetValue(itemDto.ProductId, out var product))
                    throw new NotFoundException($"Product {itemDto.ProductId} does not exist.");

                if (product.StockQuantity < itemDto.Quantity)
                    throw new BadRequestException($"Product {product.Name} has insufficient stock.");

                var existingItem = cart.Items
                    .FirstOrDefault(i => i.ProductId == itemDto.ProductId);


                if (existingItem != null)
                {
                    // Update existing
                    existingItem.Quantity = itemDto.Quantity;
                    existingItem.Product = product;
                }
                else
                {
                    // Add new
                    cart.Items.Add(new CartItem
                    {
                        ProductId = itemDto.ProductId,
                        Quantity = itemDto.Quantity,
                        Product = product
                    });
                }
            }

            cart.LastUpdated = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Refresh cart from DB to get full navigation properties (Images, Brands) for the return DTO
            // Or simpler: map the entity you just modified if EF tracking is up to date.
            return _mapper.Map<ShoppingCartDto>(cart);
        }



        public async Task ClearAsync()
        {
            var cart = await GetCartEntityAsync();

            if (cart.Items.Count != 0)
            {
                _context.CartItems.RemoveRange(cart.Items);
                cart.LastUpdated = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        private string GetCurrentUserId()
        {
            var userId = _httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedException("User is not authenticated.");

            return userId;
        }

        private async Task<ShoppingCart> GetCartEntityAsync()
        {
            var currentUserId = GetCurrentUserId();
            var cart = await _context.ShoppingCarts
                .Include(sc => sc.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Images)
                .IgnoreQueryFilters()
                .AsSplitQuery()
                .FirstOrDefaultAsync(sc => sc.UserId == currentUserId);
            if (cart == null)
            {
                cart = new ShoppingCart { UserId = currentUserId };
                _context.ShoppingCarts.Add(cart);
            }


            // 1. Identify items where the product is archived/deleted
            var invalidItems = cart.Items
                .Where(i => i.Product.IsDeleted)
                .ToList();

            // 2. Remove them from DB
            if (invalidItems.Count > 0)
            {
                _context.CartItems.RemoveRange(invalidItems);

                // Remove from memory so the mapped DTO is clean
                foreach (var item in invalidItems)
                {
                    cart.Items.Remove(item);
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Cleaned up {Count} deleted products from user cart.", invalidItems.Count);
            }

            return cart;
        }
    }
}
