using Bogus;
using ECommerce.Core.Entities;
using ECommerce.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Data.Seeders
{
    public static class OrderSeeder
    {
        public static async Task SeedAsync(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            // 1. Check if orders already exist to avoid duplicates
            if (await context.Orders.AnyAsync()) return;

            // 2. Load necessary data (Products and Customers)
            var products = await context.Products.ToListAsync();
            var customers = await userManager.GetUsersInRoleAsync("Customer");

            // Safety check: We need products and users to create orders
            if (products.Count == 0 || customers.Count == 0) return;

            var faker = new Faker();
            var orders = new List<Order>();

            // 3. Generate 50 Realistic Orders
            for (int i = 0; i < 50; i++)
            {
                // Pick a random customer
                var user = faker.PickRandom(customers);

                // Pick 1 to 5 random products for this order
                var orderProducts = faker.PickRandom(products, faker.Random.Int(1, 5)).ToList();

                // Create the Order
                var order = CreateOrder(user, orderProducts, faker);
                orders.Add(order);
            }

            // 4. Save to Database
            context.Orders.AddRange(orders);
            await context.SaveChangesAsync();
        }

        private static Order CreateOrder(ApplicationUser user, List<Product> products, Faker faker)
        {
            // --- A. Setup Basic Status & Dates ---
            // Weighted Random: 45% Delivered, 20% Shipped, etc.
            var status = faker.Random.WeightedRandom(
                new[] { OrderStatus.Pending, OrderStatus.Processing, OrderStatus.Shipped, OrderStatus.Delivered, OrderStatus.Cancelled },
                new[] { 0.1f, 0.15f, 0.2f, 0.45f, 0.1f }
            );

            var createdDate = faker.Date.Past(3); // Order made in the last 3 months

            // --- B. Setup Shipping ---
            var shippingMethod = faker.PickRandom<ShippingMethod>();

            decimal shippingFees = shippingMethod == ShippingMethod.Express ? 250m : 150m;

            // Use the user's name, but generate a fake address for the order snapshot
            // (Or you could load the user's existing address from DB if preferred)
            var shippingAddress = new OrderAddress
            {
                Street = faker.Address.StreetAddress(),
                City = faker.Address.City(),
                State = faker.Address.State(),
                PostalCode = faker.Address.ZipCode(),
                Country = "Egypt"
            };

            // --- C. Build Order Items ---
            var orderItems = new List<OrderItem>();
            foreach (var p in products)
            {
                orderItems.Add(new OrderItem
                {
                    ProductId = p.Id,
                    Quantity = faker.Random.Int(1, 3),
                    UnitPrice = p.Price // Snapshot price at time of order
                });
            }

            // --- D. Calculate Financials ---
            var subtotal = orderItems.Sum(i => i.UnitPrice * i.Quantity);
            var taxes = subtotal * 0.14m; // 14% Tax
            var totalAmount = subtotal + taxes + shippingFees;

            // --- E. Construct Order Object ---
            var order = new Order
            {
                UserId = user.Id,
                Created = createdDate,
                Updated = createdDate, // Will update based on milestones below
                Status = status,
                ShippingMethod = shippingMethod,
                ShippingAddress = shippingAddress,
                Subtotal = subtotal,
                ShippingFees = shippingFees,
                Taxes = taxes,
                TotalAmount = totalAmount,
                Items = orderItems,
                OrderTrackingMilestones = []
            };

            // --- F. Generate Milestones (History) ---

            // 1. Pending (Always happens)
            order.OrderTrackingMilestones.Add(new OrderTrackingMilestone
            {
                Status = OrderStatus.Pending,
                TimeStamp = createdDate
            });

            // If Cancelled
            if (status == OrderStatus.Cancelled)
            {
                var cancelDate = createdDate.AddHours(faker.Random.Double(1, 24));
                order.OrderTrackingMilestones.Add(new OrderTrackingMilestone
                {
                    Status = OrderStatus.Cancelled,
                    TimeStamp = cancelDate
                });
                order.Updated = cancelDate;
                return order; // Exit early
            }

            // 2. Processing
            if (status >= OrderStatus.Processing)
            {
                var processDate = createdDate.AddHours(faker.Random.Double(2, 5));
                order.OrderTrackingMilestones.Add(new OrderTrackingMilestone
                {
                    Status = OrderStatus.Processing,
                    TimeStamp = processDate
                });
                order.Updated = processDate;

                // 3. Shipped
                if (status >= OrderStatus.Shipped)
                {
                    var shipDate = processDate.AddDays(faker.Random.Double(1, 2));
                    order.OrderTrackingMilestones.Add(new OrderTrackingMilestone
                    {
                        Status = OrderStatus.Shipped,
                        TimeStamp = shipDate
                    });
                    order.Updated = shipDate;

                    // 4. Delivered
                    if (status == OrderStatus.Delivered)
                    {
                        var deliverDate = shipDate.AddDays(faker.Random.Double(2, 5));
                        order.OrderTrackingMilestones.Add(new OrderTrackingMilestone
                        {
                            Status = OrderStatus.Delivered,
                            TimeStamp = deliverDate
                        });
                        order.Updated = deliverDate;
                    }
                }
            }

            return order;
        }
    }
}