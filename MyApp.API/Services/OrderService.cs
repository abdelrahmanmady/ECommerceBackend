using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MyApp.API.Data;
using MyApp.API.DTOs.Orders;
using MyApp.API.Entities;
using MyApp.API.Enums;
using MyApp.API.Interfaces;

namespace MyApp.API.Services
{
    public class OrderService(AppDbContext context, IMapper mapper) : IOrderService
    {
        private readonly AppDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        public async Task<IEnumerable<OrderDto>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.Items)
                .ProjectTo<OrderDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<OrderDto?> GetByIdAsync(int id)
        {
            var order = await _context.Orders.Where(o => o.Id == id)
                 .ProjectTo<OrderDto>(_mapper.ConfigurationProvider)
                 .FirstOrDefaultAsync()
                 ?? throw new Exception("Invalid OrderId");
            return order;

        }

        public async Task<OrderDto> CreateAsync(CreateOrderDto dto)
        {
            if (dto.Items == null || dto.Items.Count == 0)
                throw new Exception("Order is empty.");

            var order = new Order
            {
                Created = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                TotalAmount = 0,
                Items = new List<OrderItem>()
            };

            foreach (var dtoItem in dto.Items)
            {
                var product = await _context.Products.FindAsync(dtoItem.ProductId)
                    ?? throw new Exception("Invalid ProductId");

                if (dtoItem.Quantity <= 0)
                    throw new Exception($"Invalid quantity for product {product.Name}");

                if (product.StockQuantity < dtoItem.Quantity)
                    throw new Exception($"Not enough stock for product {product.Name}");

                // Reduce stock
                product.StockQuantity -= dtoItem.Quantity;

                // Create order item
                var item = new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = dtoItem.Quantity,
                    UnitPrice = product.Price
                };

                // Use computed TotalPrice
                order.TotalAmount += item.TotalPrice;

                order.Items.Add(item);
            }

            // EF will generate OrderId and apply it to children automatically
            _context.Orders.Add(order);

            // Save everything at once → atomic transaction
            await _context.SaveChangesAsync();

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> UpdateStatusAsync(int id, UpdateOrderStatusDto dto)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id)
                ?? throw new Exception("Invalid OrderId");

            order.Status = dto.Status;
            await _context.SaveChangesAsync();
            return _mapper.Map<OrderDto>(order);
        }

        public async Task DeleteAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id)
                ?? throw new Exception("Invalid OrderId");
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
    }
}
