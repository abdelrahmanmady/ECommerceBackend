using Bogus;
using ECommerce.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Data.Seeders
{
    public static class StoreSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            await SeedBrandsAsync(context);
            await SeedCategoriesAsync(context);
            await SeedProductsAsync(context);
        }

        private static async Task SeedBrandsAsync(AppDbContext context)
        {
            if (await context.Brands.AnyAsync()) return;

            var brands = new List<Brand>
            {
                new() { Name = "Nike", Description = "Just Do It" },
                new() { Name = "Adidas", Description = "Impossible is Nothing" },
                new() { Name = "Zara", Description = "Fast Fashion & Trendy" },
                new() { Name = "H&M", Description = "Sustainable Fashion" },
                new() { Name = "Levi's", Description = "Quality Denim" },
                new() { Name = "Puma", Description = "Forever Faster" },
                new() { Name = "Gucci", Description = "Luxury Fashion" },
                new() { Name = "Uniqlo", Description = "Modern Essentials" },
                new() { Name = "Ralph Lauren", Description = "Premium Lifestyle" },
                new() { Name = "Calvin Klein", Description = "Modern Minimalist" }
            };

            context.Brands.AddRange(brands);
            await context.SaveChangesAsync();
        }

        private static async Task SeedCategoriesAsync(AppDbContext context)
        {
            if (await context.Categories.AnyAsync()) return;

            var categories = new List<Category>
            {
                new() { Name = "Men", Description = "Men's Apparel" },
                new() { Name = "Women", Description = "Women's Apparel" },
                new() { Name = "Kids", Description = "Children's Clothing" },
                new() { Name = "Shoes", Description = "Footwear & Sneakers" },
                new() { Name = "Accessories", Description = "Hats, Belts, Scarves" },
                new() { Name = "Sportswear", Description = "Activewear & Gym Gear" }
            };

            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();
        }

        private static async Task SeedProductsAsync(AppDbContext context)
        {
            if (await context.Products.AnyAsync()) return;

            var brands = await context.Brands.ToListAsync();
            var categories = await context.Categories.ToListAsync();

            var faker = new Faker();

            var adjectives = new[] { "Cotton", "Slim Fit", "Oversized", "Vintage", "Denim", "Leather", "Casual", "Summer", "Wool", "Striped", "Classic", "Breathable", "Athletic" };
            var apparel = new[] { "T-Shirt", "Jeans", "Jacket", "Hoodie", "Dress", "Sneakers", "Skirt", "Blazer", "Sweater", "Joggers", "Shorts", "Coat" };

            var products = new List<Product>();

            // Generate 100 Products
            for (int i = 1; i <= 100; i++)
            {
                var brand = faker.PickRandom(brands);
                var category = faker.PickRandom(categories);
                var name = $"{brand.Name} {faker.PickRandom(adjectives)} {faker.PickRandom(apparel)}";

                // Create 4 distinct images for this product
                // We append -1, -2, -3, -4 to the seed to ensure different images
                var productImages = new List<ProductImage>
                {
                    new() { IsMain = true, ImageUrl = $"https://picsum.photos/seed/{i}-1/300/300" },  // Main View
                    new() { IsMain = false, ImageUrl = $"https://picsum.photos/seed/{i}-2/300/300" }, // Side View
                    new() { IsMain = false, ImageUrl = $"https://picsum.photos/seed/{i}-3/300/300" }, // Back View
                    new() { IsMain = false, ImageUrl = $"https://picsum.photos/seed/{i}-4/300/300" }  // Detail View
                };

                products.Add(new Product
                {
                    Name = name,
                    Description = faker.Commerce.ProductDescription(),
                    Price = decimal.Parse(faker.Commerce.Price(10, 10000)),
                    StockQuantity = faker.Random.Int(0, 100),
                    BrandId = brand.Id,
                    CategoryId = category.Id,
                    IsFeatured = faker.Random.Bool(0.2f),
                    Images = productImages // Assign the list of 4 images
                });
            }

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }
    }
}