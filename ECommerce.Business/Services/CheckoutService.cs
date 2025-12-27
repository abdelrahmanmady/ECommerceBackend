using AutoMapper;
using AutoMapper.QueryableExtensions;
using ECommerce.Business.DTOs.Checkout.Requests;
using ECommerce.Business.DTOs.Checkout.Responses;
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

        public async Task<CheckoutPreviewResponse> GetCheckoutPreviewAsync(ShippingMethod shippingMethod)
        {
            var currentUserId = GetCurrentUserId();

            var cart = await _context.ShoppingCarts
                .Include(sc => sc.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Images)
                .IgnoreQueryFilters()
                .AsSplitQuery()
                .FirstOrDefaultAsync(sc => sc.UserId == currentUserId)
                ?? throw new NotFoundException("Shopping Cart does not exist for current user.");

            if (cart.Items.Count == 0)
                throw new BadRequestException("Cannot checkout. Your cart is empty.");

            var invalidItems = cart.Items
                .Where(i => i.Product.IsDeleted)
                .ToList();


            if (invalidItems.Count > 0)
            {
                _context.CartItems.RemoveRange(invalidItems);

                foreach (var item in invalidItems)
                {
                    cart.Items.Remove(item);
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Cleaned up {Count} deleted products from user cart.", invalidItems.Count);
            }

            var calculations = ProcessShoppingCart(cart, shippingMethod);

            var checkoutPreviewDto = _mapper.Map<CheckoutPreviewResponse>(cart);

            checkoutPreviewDto.Subtotal = calculations.Subtotal;
            checkoutPreviewDto.ShippingFees = calculations.ShippingFees;
            checkoutPreviewDto.Taxes = calculations.Taxes;
            checkoutPreviewDto.Total = calculations.Total;
            checkoutPreviewDto.EstimatedDeliveryDateStart = DateTime.UtcNow.AddDays(shippingMethod == ShippingMethod.Express ? 2 : 5);
            checkoutPreviewDto.EstimatedDeliveryDateEnd = DateTime.UtcNow.AddDays(shippingMethod == ShippingMethod.Express ? 3 : 7);

            return checkoutPreviewDto;
        }

        public async Task<OrderConfirmationResponse> CheckoutAsync(CheckoutRequest checkoutRequest)
        {
            var currentUserId = GetCurrentUserId();

            var cart = await _context.ShoppingCarts
                .Include(sc => sc.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Images)
                .Include(sc => sc.User)
                .IgnoreQueryFilters()
                .AsSplitQuery()
                .FirstOrDefaultAsync(sc => sc.UserId == currentUserId)
                ?? throw new NotFoundException("Shopping Cart does not exist for current user.");

            if (cart.Items.Count == 0)
                throw new BadRequestException("Cannot checkout. Your cart is empty.");

            var invalidItems = cart.Items
                .Where(i => i.Product.IsDeleted)
                .ToList();


            if (invalidItems.Count > 0)
            {
                _context.CartItems.RemoveRange(invalidItems);
                await _context.SaveChangesAsync();

                throw new ConflictException("Some items in your cart are no longer available. Your cart has been updated. Please review before paying.");
            }

            var shippingAddress = await _context.Addresses
                .AsNoTracking()
                .Where(a => a.Id == checkoutRequest.ShippingAddressId && a.UserId == currentUserId)
                .ProjectTo<OrderAddress>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException("Shipping Address not found.");

            var calculations = ProcessShoppingCart(cart, checkoutRequest.ShippingMethod);

            //create order
            var orderToCreate = new Order
            {
                Status = OrderStatus.Pending,
                Subtotal = calculations.Subtotal,
                ShippingFees = calculations.ShippingFees,
                Taxes = calculations.Taxes,
                TotalAmount = calculations.Total,
                ShippingMethod = checkoutRequest.ShippingMethod,
                PaymentMethod = checkoutRequest.PaymentMethod,
                ShippingAddress = shippingAddress,
                Items = [],
                UserId = currentUserId,
                User = cart.User,
                OrderTrackingMilestones = [],
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow

            };

            foreach (var cartItem in cart.Items)
            {
                if (cartItem.Quantity > cartItem.Product.StockQuantity)
                    throw new BadRequestException($"Not enough stock for {cartItem.Product.Name}. Available: {cartItem.Product.StockQuantity}");
                cartItem.Product.StockQuantity -= cartItem.Quantity;

                orderToCreate.Items.Add(_mapper.Map<OrderItem>(cartItem));
            }

            orderToCreate.OrderTrackingMilestones.Add(new OrderTrackingMilestone
            {
                Status = OrderStatus.Pending,
                TimeStamp = DateTime.UtcNow,
            });

            _context.Orders.Add(orderToCreate);
            _context.CartItems.RemoveRange(cart.Items);
            cart.Updated = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
                if (_logger.IsEnabled(LogLevel.Information))
                    _logger.LogInformation("User {userId} placed an order {orderId} at {datetime}", currentUserId, orderToCreate.Id, DateTime.UtcNow);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConflictException("Stock changed during checkout. Please try again.");
            }

            return _mapper.Map<OrderConfirmationResponse>(orderToCreate);
        }

        //Helper Methods
        private string GetCurrentUserId()
        {
            var userId = _httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedException("User is not authenticated.");

            return userId;
        }

        private static OrderCalculations ProcessShoppingCart(ShoppingCart cart, ShippingMethod shippingMethod)
        {
            var subtotal = cart.Items.Sum(ci => ci.Product.Price * ci.Quantity);
            var shippingFees = shippingMethod == ShippingMethod.Express ? 250m : 150m;
            var taxes = 0.14m * subtotal;
            var total = subtotal + shippingFees + taxes;

            return new OrderCalculations(subtotal, shippingFees, taxes, total);
        }

        //Helper Records
        private record OrderCalculations(decimal Subtotal, decimal ShippingFees, decimal Taxes, decimal Total);


    }
}
