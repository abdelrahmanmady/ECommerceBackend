using AutoMapper;
using AutoMapper.QueryableExtensions;
using ECommerce.Business.DTOs.Checkout;
using ECommerce.Business.DTOs.Orders.Profile;
using ECommerce.Business.Interfaces;
using ECommerce.Core.Entities;
using ECommerce.Core.Enums;
using ECommerce.Core.Exceptions;
using ECommerce.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ECommerce.Business.Services
{
    public class CheckoutService(
        AppDbContext context,
        IMapper mapper,
        ILogger<CheckoutService> logger,
        IHttpContextAccessor httpContext) : ICheckoutService
    {
        private readonly AppDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<CheckoutService> _logger = logger;
        private readonly IHttpContextAccessor _httpContext = httpContext;

        public async Task<CheckoutPreviewDto> GetCheckoutPreviewAsync(ShippingMethod shippingMethod)
        {
            var currentUserId = GetCurrentUserId();
            var cart = await GetCartEntityAsync(currentUserId);
            if (cart.Items.Count == 0)
                throw new BadRequestException("Cannot checkout. Your cart is empty.");
            var calculations = ProcessShoppingCart(cart, shippingMethod);

            var checkoutPreviewDto = _mapper.Map<CheckoutPreviewDto>(cart);

            checkoutPreviewDto.Subtotal = calculations.Subtotal;
            checkoutPreviewDto.ShippingFees = calculations.ShippingFees;
            checkoutPreviewDto.Taxes = calculations.Taxes;
            checkoutPreviewDto.Total = calculations.Total;

            return checkoutPreviewDto;
        }

        public async Task<OrderDto> CheckoutAsync(CheckoutDto dto)
        {
            var currentUserId = GetCurrentUserId();

            var cart = await GetCartEntityAsync(currentUserId);

            if (cart.Items.Count == 0)
                throw new BadRequestException("Cannot checkout. Your cart is empty.");

            var shippingAddress = await _context.Addresses
                .AsNoTracking()
                .Where(a => a.Id == dto.ShippingAddressId && a.UserId == currentUserId)
                .ProjectTo<OrderAddress>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException("Shipping Address not found.");

            var calculations = ProcessShoppingCart(cart, dto.ShippingMethod);

            //create order
            var orderToCreate = new Order
            {
                Status = OrderStatus.Pending,
                Subtotal = calculations.Subtotal,
                ShippingFees = calculations.ShippingFees,
                Taxes = calculations.Taxes,
                TotalAmount = calculations.Total,
                ShippingMethod = dto.ShippingMethod,
                PaymentMethod = dto.PaymentMethod,
                ShippingAddress = shippingAddress,
                Items = [],
                UserId = currentUserId,
                OrderTrackingMilestones = []
            };

            //Loop over the cart items => validate stock => add them to order
            foreach (var cartItem in cart.Items)
            {
                //product stock check
                if (cartItem.Quantity > cartItem.Product.StockQuantity)
                    throw new BadRequestException($"Not enough stock for {cartItem.Product.Name}. Available: {cartItem.Product.StockQuantity}");
                cartItem.Product.StockQuantity -= cartItem.Quantity;

                orderToCreate.Items.Add(_mapper.Map<OrderItem>(cartItem));
            }

            orderToCreate.OrderTrackingMilestones.Add(new OrderTrackingMilestone
            {
                Status = OrderStatus.Pending,
            });

            _context.Orders.Add(orderToCreate);
            _context.CartItems.RemoveRange(cart.Items);
            cart.LastUpdated = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConflictException("Stock changed during checkout. Please try again.");
            }

            return _mapper.Map<OrderDto>(orderToCreate);
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
                .Include(sc => sc.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Images)
                .IgnoreQueryFilters()
                .AsSplitQuery()
                .FirstOrDefaultAsync(sc => sc.UserId == userId);
            if (cart == null)
            {
                cart = new ShoppingCart { UserId = userId };
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

        private static OrderCalculations ProcessShoppingCart(ShoppingCart cart, ShippingMethod shippingMethod)
        {
            var subtotal = cart.Items.Sum(ci => ci.Product.Price * ci.Quantity);
            var shippingFees = shippingMethod == ShippingMethod.Express ? 250m : 150m;
            var taxes = 0.14m * subtotal;
            var total = subtotal + shippingFees + taxes;

            return new OrderCalculations(subtotal, shippingFees, taxes, total);
        }

        private record OrderCalculations(decimal Subtotal, decimal ShippingFees, decimal Taxes, decimal Total);


    }
}
