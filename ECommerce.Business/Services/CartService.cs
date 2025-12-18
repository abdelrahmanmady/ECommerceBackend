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
        {
            var currentUserId = GetCurrentUserId();
            var cart = await GetCartEntityAsync(currentUserId);
            return _mapper.Map<ShoppingCartDto>(cart);
        }

        public async Task<ShoppingCartDto> UpdateAsync(UpdateShoppingCartDto dto)
        {
            if (dto.Items is null || dto.Items.Count == 0)
                throw new BadRequestException("No Items found in the incoming request.");

            var currentUserId = GetCurrentUserId();
            var cart = await GetCartEntityAsync(currentUserId);

            foreach (var cartItem in dto.Items)
            {
                var cartItemToUpdate = await _context.CartItems
                    .FirstOrDefaultAsync(ci => ci.ProductId == cartItem.ProductId && ci.ShoppingCartId == cart.Id);

                var product = await _context.Products.FindAsync(cartItem.ProductId)
                    ?? throw new NotFoundException("Product does not exist.");

                if (cartItemToUpdate is null)
                {
                    cartItemToUpdate = new CartItem { ProductId = cartItem.ProductId };
                    cart.Items.Add(cartItemToUpdate);
                }

                //check stock
                if (product.StockQuantity < cartItem.Quantity)
                    throw new BadRequestException($"Added Quantity exceeds product stock, In Stock : {product.StockQuantity}");

                if (cartItem.Quantity <= 0)
                {
                    _context.CartItems.Remove(cartItemToUpdate);
                    continue;
                }
                cartItemToUpdate.Quantity = cartItem.Quantity;
            }
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
                .ThenInclude(p => p.Images)
                .AsSplitQuery()
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
