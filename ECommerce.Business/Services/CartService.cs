using AutoMapper;
using ECommerce.Business.DTOs.ShoppingCart.Requests;
using ECommerce.Business.DTOs.ShoppingCart.Responses;
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

        public async Task<CartResponse> GetCartAsync()
        {
            var currentUserId = GetCurrentUserId();

            var cart = await _context.ShoppingCarts
                .IgnoreQueryFilters()
                .Where(sc => sc.UserId == currentUserId)
                .Include(sc => sc.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Images)
                .AsSplitQuery()
                .FirstOrDefaultAsync();

            if (cart == null)
            {
                return new CartResponse();
            }

            var invalidItems = cart.Items
                .Where(i => i.Product.IsDeleted || i.Product.StockQuantity == 0)
                .ToList();

            List<string> warnings = [];

            if (invalidItems.Count > 0)
            {
                _context.CartItems.RemoveRange(invalidItems);

                foreach (var item in invalidItems)
                {
                    cart.Items.Remove(item);
                    if (item.Product.IsDeleted)
                        warnings.Add($"Item '{item.Product.Name}' got removed because it is no longer available.");
                    else
                        warnings.Add($"Item '{item.Product.Name} got removed because it is out of stock.");
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("Cleaned up {Count} deleted products from user cart.", invalidItems.Count);
            }

            var response = _mapper.Map<CartResponse>(cart);
            response.Warnings = warnings;
            return response;

        }

        public async Task<CartResponse> UpdateCartAsync(UpdateCartRequest updateCartRequest)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var currentUserId = GetCurrentUserId();

                var cart = await _context.ShoppingCarts
                    .IgnoreQueryFilters()
                    .Where(sc => sc.UserId == currentUserId)
                    .Include(c => c.Items)
                    .FirstOrDefaultAsync();

                if (cart == null)
                {
                    cart = new ShoppingCart { UserId = currentUserId };
                    _context.ShoppingCarts.Add(cart);
                }

                var incomingProductIds = updateCartRequest.Items.Select(i => i.ProductId).Distinct().ToList();

                cart.Items.RemoveAll(i => !incomingProductIds.Contains(i.ProductId));

                var productsDict = await _context.Products
                    .Include(p => p.Images)
                    .Where(p => incomingProductIds.Contains(p.Id))
                    .IgnoreQueryFilters()
                    .ToDictionaryAsync(p => p.Id);

                List<string> warnings = [];

                foreach (var itemDto in updateCartRequest.Items)
                {
                    if (!productsDict.TryGetValue(itemDto.ProductId, out var product))
                    {
                        warnings.Add($"Item {itemDto.ProductId} not added because it is no longer exists.");
                        continue;
                    }
                    if (product.IsDeleted)
                    {
                        warnings.Add($"Item '{product.Name}' not added because it is no longer available.");
                        continue;
                    }
                    if (product.StockQuantity == 0)
                    {
                        warnings.Add($"Item '{product.Name}' not added because it is out of stock.");
                        continue;
                    }
                    if (product.StockQuantity < itemDto.Quantity)
                    {
                        itemDto.Quantity = product.StockQuantity;
                        warnings.Add($"Quantity for '{product.Name}' adjusted to {product.StockQuantity} due to stock limits.");
                    }

                    var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == itemDto.ProductId);

                    if (existingItem != null)
                    {
                        existingItem.Quantity = itemDto.Quantity;
                        existingItem.Product = product;
                    }
                    else
                    {
                        cart.Items.Add(new CartItem
                        {
                            ProductId = itemDto.ProductId,
                            Quantity = itemDto.Quantity,
                            Product = product
                        });
                    }
                }

                cart.Updated = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var response = _mapper.Map<CartResponse>(cart);
                response.Warnings = warnings;
                return response;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task ClearCartAsync()
        {
            var currentUserId = GetCurrentUserId();

            await _context.CartItems
                .Where(ci => ci.ShoppingCart.UserId == currentUserId)
                .ExecuteDeleteAsync();

            await _context.ShoppingCarts
                .Where(sc => sc.UserId == currentUserId)
                .ExecuteUpdateAsync(x => x.SetProperty(sc => sc.Updated, DateTime.UtcNow));
        }

        //Helper Methods
        private string GetCurrentUserId()
        {
            var userId = _httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedException("User is not authenticated.");

            else if (_context.Users.IgnoreQueryFilters().Any(u => u.Id == userId && u.IsDeleted))
                throw new UnauthorizedException("User is no longer active.");

            return userId;
        }
    }
}
