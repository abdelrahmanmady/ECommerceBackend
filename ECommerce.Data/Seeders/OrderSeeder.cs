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
            if (await context.Orders.AnyAsync()) return;

            var products = await context.Products.ToListAsync();
            var users = await userManager.GetUsersInRoleAsync("Customer");
            var faker = new Faker();
            var orders = new List<Order>();

            // Generate 100 Orders
            for (int i = 0; i < 100; i++)
            {
                var user = faker.PickRandom(users);

                // Pick products
                int itemsCount = faker.Random.Int(1, 5);
                var orderProducts = faker.PickRandom(products, itemsCount).ToList();

                var order = CreateOrder(user, orderProducts, faker);
                orders.Add(order);
            }

            context.Orders.AddRange(orders);
            await context.SaveChangesAsync();
        }

        private static Order CreateOrder(ApplicationUser customer, List<Product> products, Faker faker)
        {
            // 1. Determine the Order's Final Status randomly
            // Weighted Random to have more "completed" orders for charts
            var status = faker.Random.WeightedRandom(
                new[] { OrderStatus.Pending, OrderStatus.Processing, OrderStatus.Shipped, OrderStatus.Delivered, OrderStatus.Cancelled },
                new[] { 0.1f, 0.15f, 0.2f, 0.45f, 0.1f } // 45% Delivered, 10% Pending, etc.
            );

            var shippingMethod = faker.PickRandom<ShippingMethod>();

            // 2. Set Creation Date (e.g., up to 90 days ago)
            var createdDate = faker.Date.Recent(90);

            var address = customer.Addresses?.FirstOrDefault()
                          ?? new Address { Street = "Default St", City = "Cairo", Country = "EG" };

            var order = new Order
            {
                UserId = customer.Id,
                Created = createdDate,
                Updated = DateTime.UtcNow, // Will be updated to last milestone date below
                Status = status,
                ShippingMethod = shippingMethod,
                ShippingAddress = new OrderAddress
                {
                    Street = address.Street,
                    City = address.City,
                    State = address.State ?? "Cairo",
                    Country = address.Country,
                    PostalCode = address.PostalCode ?? "12345"
                },
                Items = [],
                OrderTrackingMilestones = []
            };

            // Add Items
            foreach (var p in products)
            {
                order.Items.Add(new OrderItem
                {
                    ProductId = p.Id,
                    Quantity = faker.Random.Int(1, 3),
                    UnitPrice = p.Price
                });
            }

            // Financials
            order.Subtotal = order.Items.Sum(i => i.UnitPrice * i.Quantity);
            order.ShippingFees = shippingMethod == ShippingMethod.Express ? 250 : 100;
            order.Taxes = order.Subtotal * 0.14m;
            order.TotalAmount = order.Subtotal + order.ShippingFees + order.Taxes;

            // ---------------------------------------------------------
            // 3. LOGICAL MILESTONE GENERATION (The Fix)
            // ---------------------------------------------------------

            // Every order starts as Pending
            order.OrderTrackingMilestones.Add(new OrderTrackingMilestone
            {
                Status = OrderStatus.Pending,
                TimeStamp = createdDate
            });

            // If status is "higher" than Pending, add Processing
            if (status != OrderStatus.Pending && status != OrderStatus.Cancelled)
            {
                // Processing happens 1-4 hours after creation
                var processDate = createdDate.AddHours(faker.Random.Double(1, 4));
                order.OrderTrackingMilestones.Add(new OrderTrackingMilestone
                {
                    Status = OrderStatus.Processing,
                    TimeStamp = processDate
                });

                // If status is Shipped or Delivered, add Shipped
                if (status == OrderStatus.Shipped || status == OrderStatus.Delivered)
                {
                    // Shipping happens 1-2 days after processing
                    var shipDate = processDate.AddDays(faker.Random.Double(1, 2));
                    order.OrderTrackingMilestones.Add(new OrderTrackingMilestone
                    {
                        Status = OrderStatus.Shipped,
                        TimeStamp = shipDate
                    });

                    // If status is Delivered, add Delivered
                    if (status == OrderStatus.Delivered)
                    {
                        // Delivery happens 2-5 days after shipping
                        var deliverDate = shipDate.AddDays(faker.Random.Double(2, 5));
                        order.OrderTrackingMilestones.Add(new OrderTrackingMilestone
                        {
                            Status = OrderStatus.Delivered,
                            TimeStamp = deliverDate
                        });

                        // Update the Order.Updated to reflect the final event
                        order.Updated = deliverDate;
                    }
                    else
                    {
                        order.Updated = shipDate;
                    }
                }
                else
                {
                    order.Updated = processDate;
                }
            }

            // Handle Canceled scenario
            if (status == OrderStatus.Cancelled)
            {
                // Cancellation happens some time after creation (e.g., 5 hours later)
                var cancelDate = createdDate.AddHours(faker.Random.Double(2, 24));
                order.OrderTrackingMilestones.Add(new OrderTrackingMilestone
                {
                    Status = OrderStatus.Cancelled,
                    TimeStamp = cancelDate
                });
                order.Updated = cancelDate;
            }

            return order;
        }
    }
}