using AutoMapper;
using AutoMapper.QueryableExtensions;
using ECommerce.Business.DTOs.Orders;
using ECommerce.Business.DTOs.Orders.Admin;
using ECommerce.Business.DTOs.Orders.Management;
using ECommerce.Business.DTOs.Pagination;
using ECommerce.Business.Interfaces;
using ECommerce.Core.Entities;
using ECommerce.Core.Enums;
using ECommerce.Core.Exceptions;
using ECommerce.Core.Specifications;
using ECommerce.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ECommerce.Business.Services
{
    public class OrderService(
        AppDbContext context,
        IMapper mapper,
        ILogger<OrderService> logger,
        IHttpContextAccessor httpContext
        ) : IOrderService
    {
        private readonly AppDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<OrderService> _logger = logger;
        private readonly IHttpContextAccessor _httpContext = httpContext;


        public async Task<IEnumerable<OrderDto>> GetOrdersForCustomerAsync()
        {
            var currentUserId = GetCurrentUserId();

            var query = _context.Orders.AsNoTracking();

            if (!IsAdmin())
            {
                query = query.Where(o => o.UserId == currentUserId);
            }

            var orders = await query
                .ProjectTo<OrderDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
            return orders;
        }

        public async Task<PagedResponseDto<AdminOrderDto>> GetOrdersForAdminAsync(AdminOrderSpecParams specParams)
        {
            var query = _context.Orders.AsNoTracking().AsQueryable();

            //Search
            if (!string.IsNullOrEmpty(specParams.Search))
            {
                var isNumeric = int.TryParse(specParams.Search, out int searchId);
                query = query.Where(o => (isNumeric && o.Id == searchId) ||
                $"{o.User.FirstName} {o.User.LastName}".ToLower().Contains(specParams.Search.ToLower()));
            }

            //Filter
            if (specParams.Status.HasValue)
            {
                query = query.Where(o => o.Status == specParams.Status.Value);
            }

            //Sort
            query = specParams.Sort switch
            {
                "oldest" => query.OrderBy(o => o.Created),
                "highestTotal" => query.OrderByDescending(o => o.TotalAmount),
                "lowestTotal" => query.OrderBy(o => o.TotalAmount),
                _ => query.OrderByDescending(o => o.Created)
            };

            //Pagination
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((specParams.PageIndex - 1) * specParams.PageSize)
                .Take(specParams.PageSize)
                .ProjectTo<AdminOrderDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new PagedResponseDto<AdminOrderDto>
            {
                PageIndex = specParams.PageIndex,
                PageSize = specParams.PageSize,
                TotalCount = totalCount,
                Items = items
            };
        }

        public async Task<OrderDto> GetByIdCustomerAsync(int id)
        {
            var currentUserId = GetCurrentUserId();

            var order = await _context.Orders
                .AsNoTracking()
                .Where(o => o.UserId == currentUserId && o.Id == id)
                .ProjectTo<OrderDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException("Order does not exist.");
            return order;

        }

        public async Task<AdminOrderDetailsDto> GetByIdAdminAsync(int id)
        {
            var order = await _context.Orders
                .AsNoTracking()
                .Where(o => o.Id == id)
                .ProjectTo<AdminOrderDetailsDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException("Order does not exist.");
            return order;
        }



        public async Task<AdminOrderDetailsDto> UpdateStatusAsync(int id, AdminUpdateOrderDto dto)
        {


            var orderToUpdate = await _context.Orders
                .FindAsync(id)
                ?? throw new NotFoundException("Order does not exist.");

            if (dto.Status == OrderStatus.Canceled && orderToUpdate.Status != OrderStatus.Canceled)
            {
                foreach (var item in orderToUpdate.Items)
                {
                    item.Product.StockQuantity += item.Quantity;
                }
            }
            orderToUpdate.Status = dto.Status;

            if (orderToUpdate.Status == OrderStatus.Pending || orderToUpdate.Status == OrderStatus.Processing)
            {
                var address = await _context.Addresses.AsNoTracking().FirstOrDefaultAsync(a => a.Id == dto.AddressId);
                if (address?.UserId == orderToUpdate.UserId)
                {
                    orderToUpdate.ShippingAddress = new OrderAddress()
                    {
                        Street = address.Street,
                        City = address.City,
                        State = address.State,
                        PostalCode = address.PostalCode,
                        Country = address.Country
                    };
                }
                else
                {
                    throw new NotFoundException("Address does not exist.");
                }

            }

            await _context.SaveChangesAsync();

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Order updated with id = {orderId}.", orderToUpdate.Id);

            return _mapper.Map<AdminOrderDetailsDto>(orderToUpdate);
        }

        public async Task DeleteAsync(int id)
        {
            var currentUserId = GetCurrentUserId();
            var orderToDelete = await _context.Orders
                .Where(o => o.Id == id && (IsAdmin() || o.UserId == currentUserId))
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException("Order does not exist.");

            _context.Orders.Remove(orderToDelete);
            await _context.SaveChangesAsync();

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Order deleted with id = {orderId}.", orderToDelete.Id);
        }

        public async Task<OrderDto> CheckoutAsync(CheckoutDto dto)
        {
            //get current user
            var currentUserId = GetCurrentUserId();

            // get User's Cart (Include Product info for Price/Stock validation)
            var cart = await _context.ShoppingCarts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(sc => sc.UserId == currentUserId);

            if (cart == null || cart.Items.Count == 0)
                throw new BadRequestException("Cannot checkout. Your cart is empty.");

            //get shipping address of user
            var shippingAddress = await _context.Addresses
                .AsNoTracking()
                .Where(a => a.Id == dto.ShippingAddressId && a.UserId == currentUserId)
                .ProjectTo<OrderAddress>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync()
                ?? throw new NotFoundException("Shipping Address not found.");
            //create order
            var orderToCreate = new Order
            {
                UserId = currentUserId,
                Status = OrderStatus.Pending,
                ShippingAddress = shippingAddress,
                Items = []
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

            orderToCreate.TotalAmount = orderToCreate.Items.Sum(i => i.TotalPrice);
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

        private bool IsAdmin() => _httpContext?.HttpContext?.User.IsInRole("Admin") ?? false;

    }
}
