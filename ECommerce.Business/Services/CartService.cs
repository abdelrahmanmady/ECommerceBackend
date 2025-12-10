using AutoMapper;
using ECommerce.Business.DTOs.ShoppingCart;
using ECommerce.Business.Interfaces;
using ECommerce.Core.Entities;
using ECommerce.Core.Exceptions;
using ECommerce.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ECommerce.Business.Services
{
    public class CartService(
        AppDbContext context,
        IMapper mapper,
        IHttpContextAccessor httpContext) : ICartService
    {
        private readonly AppDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly IHttpContextAccessor _httpContext = httpContext;

        public async Task<ShoppingCartDto> GetAsync()
        {
            var currentUserId = GetCurrentUserId();
            var cart = await GetCartEntityAsync(currentUserId);
            return _mapper.Map<ShoppingCartDto>(cart);
        }



        public async Task<ShoppingCartDto> AddItemAsync(int productId)
        {
            var currentUserId = GetCurrentUserId();
            var cart = await GetCartEntityAsync(currentUserId);

            var product = await _context.Products.FindAsync(productId)
                ?? throw new NotFoundException("Product does not exist.");

            if (product.StockQuantity == 0)
                throw new BadRequestException("Out of stock.");

            var exisitngItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (exisitngItem is not null)
            {
                exisitngItem.Quantity++;
                if (product.StockQuantity < exisitngItem.Quantity)
                    throw new BadRequestException($"Cannot add more. You already have {exisitngItem.Quantity - 1} in cart.");
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = productId,
                    Quantity = 1
                });
            }

            cart.LastUpdated = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return _mapper.Map<ShoppingCartDto>(cart);
        }

        public async Task<ShoppingCartDto> RemoveItemAsync(int productId)
        {
            var currentUserId = GetCurrentUserId();
            var cart = await GetCartEntityAsync(currentUserId);

            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId)
                ?? throw new NotFoundException("Item does not exist in cart.");

            item.Quantity--;

            if (item.Quantity == 0)
                _context.CartItems.Remove(item);

            cart.LastUpdated = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return _mapper.Map<ShoppingCartDto>(cart);
        }

        public async Task ClearAsync()
        {
            var currentUserId = GetCurrentUserId();
            var cart = await GetCartEntityAsync(currentUserId);

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

        private async Task<ShoppingCart> GetCartEntityAsync(string userId)
        {
            var cart = await _context.ShoppingCarts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new ShoppingCart { UserId = userId };
                _context.ShoppingCarts.Add(cart);
                await _context.SaveChangesAsync();
            }
            return cart;
        }
    }
}
