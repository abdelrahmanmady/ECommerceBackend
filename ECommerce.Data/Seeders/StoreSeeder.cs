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
                new() { Name = "Nike", Description = "Just Do It" ,Created = DateTime.UtcNow, Updated = DateTime.UtcNow},
                new() { Name = "Adidas", Description = "Impossible is Nothing" ,Created = DateTime.UtcNow, Updated = DateTime.UtcNow},
                new() {Name = "Zara", Description = "Fast Fashion & Trendy", Created = DateTime.UtcNow, Updated = DateTime.UtcNow},
                new() {Name = "H&M", Description = "Sustainable Fashion", Created = DateTime.UtcNow, Updated = DateTime.UtcNow},
                new() {Name = "Levi's", Description = "Quality Denim", Created = DateTime.UtcNow, Updated = DateTime.UtcNow},
                new() {Name = "Puma", Description = "Forever Faster", Created = DateTime.UtcNow, Updated = DateTime.UtcNow},
                new() {Name = "Gucci", Description = "Luxury Fashion", Created = DateTime.UtcNow, Updated = DateTime.UtcNow},
                new() {Name = "Uniqlo", Description = "Modern Essentials", Created = DateTime.UtcNow, Updated = DateTime.UtcNow},
                new() {Name = "Ralph Lauren", Description = "Premium Lifestyle", Created = DateTime.UtcNow, Updated = DateTime.UtcNow},
                new() {Name = "Calvin Klein", Description = "Modern Minimalist", Created = DateTime.UtcNow, Updated = DateTime.UtcNow}
            };

            context.Brands.AddRange(brands);
            await context.SaveChangesAsync();
        }

        public static async Task SeedCategoriesAsync(AppDbContext context)
        {
            if (await context.Categories.AnyAsync()) return;

            var categories = new List<Category>();

            // Define Roots
            var men = new Category { Name = "Men", HierarchyPath = "Men", Created = DateTime.UtcNow, Updated = DateTime.UtcNow };
            var women = new Category { Name = "Women", HierarchyPath = "Women", Created = DateTime.UtcNow, Updated = DateTime.UtcNow };
            var kids = new Category { Name = "Kids", HierarchyPath = "Kids", Created = DateTime.UtcNow, Updated = DateTime.UtcNow };

            categories.AddRange(new[] { men, women, kids });

            // --- MEN ---
            var mClothing = CreateChild(men, "Clothing");
            var mFootwear = CreateChild(men, "Footwear");
            var mAccess = CreateChild(men, "Accessories");
            categories.AddRange(new[] { mClothing, mFootwear, mAccess });

            // Men Leaves
            categories.Add(CreateChild(mClothing, "T-Shirts"));
            categories.Add(CreateChild(mClothing, "Jeans"));
            categories.Add(CreateChild(mFootwear, "Sneakers"));
            categories.Add(CreateChild(mFootwear, "Formal Shoes"));
            categories.Add(CreateChild(mAccess, "Watches"));

            // --- WOMEN ---
            var wClothing = CreateChild(women, "Clothing");
            var wFootwear = CreateChild(women, "Footwear");
            var wAccess = CreateChild(women, "Accessories");
            categories.AddRange(new[] { wClothing, wFootwear, wAccess });

            // Women Leaves
            categories.Add(CreateChild(wClothing, "Dresses"));
            categories.Add(CreateChild(wClothing, "Tops"));
            categories.Add(CreateChild(wFootwear, "Heels"));
            categories.Add(CreateChild(wAccess, "Handbags"));

            // --- KIDS ---
            var kClothing = CreateChild(kids, "Clothing");
            var kFootwear = CreateChild(kids, "Footwear");
            categories.AddRange(new[] { kClothing, kFootwear });

            categories.Add(CreateChild(kClothing, "Baby Wear"));
            categories.Add(CreateChild(kFootwear, "School Shoes"));

            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();
        }

        // Helper Function
        private static Category CreateChild(Category parent, string name)
        {
            return new Category
            {
                Name = name,
                Parent = parent,
                HierarchyPath = $"{parent.HierarchyPath}\\{name}",
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
            };
        }

        private static async Task SeedProductsAsync(AppDbContext context)
        {
            if (await context.Products.AnyAsync()) return;

            var brands = await context.Brands.ToListAsync();
            var categories = await context.Categories.ToListAsync();
            var faker = new Faker();

            var products = new List<Product>();

            var productBlueprints = new Dictionary<string, string[]>
            {
                // --- MEN ---
                { "T-Shirts", new[] { "Essential Crew Neck", "Graphic Print Tee", "Oversized Cotton T-Shirt", "Vintage Logo Tee", "Performance Polo" } },
                { "Jeans", new[] { "Slim Fit Denim", "Straight Leg Jeans", "Ripped Wash Jeans", "Tapered Cargo Pants", "Classic Blue Jeans" } },
                { "Sneakers", new[] { "Air Runner 500", "Court Vision Low", "Ultralight Jogger", "Retro High-Top", "Streetwear Skate Shoe" } },
                { "Formal Shoes", new[] { "Leather Oxford", "Classic Brogues", "Suede Loafers", "Derby Shoes" } },
                { "Watches", new[] { "Chronograph Steel", "Minimalist Leather Watch", "Digital Sport Watch", "Automatic Diver" } },

                // --- WOMEN ---
                { "Dresses", new[] { "Floral Summer Dress", "Elegant Evening Gown", "Casual Midi Dress", "Wrap Dress", "Knitted Sweater Dress" } },
                { "Tops", new[] { "Silk Blouse", "Crop Top", "Linen Button-Up", "Chiffon Tunic" } },
                { "Heels", new[] { "Classic Stilettos", "Block Heel Sandals", "Pointed Toe Pumps", "Ankle Strap Heels" } },
                { "Handbags", new[] { "Leather Tote", "Quilted Crossbody", "Clutch Bag", "Designer Shoulder Bag" } },

                // --- KIDS ---
                { "Baby Wear", new[] { "Cotton Onesie", "Soft Knit Set", "Animal Print Romper", "Cozy Sleepsuit" } },
                { "School Shoes", new[] { "Durable Leather Shoes", "Velcro Strap Sneakers", "Black Formal Shoes" } }
            };

            // Loop through our defined blueprints and create products for matching categories
            foreach (var blueprint in productBlueprints)
            {
                // Find category in DB by Name (e.g. "Sneakers" might be under Men or Kids)
                // We use ToList() to handle potential duplicates if names are reused across trees
                var targetCategories = categories.Where(c => c.Name == blueprint.Key).ToList();

                foreach (var category in targetCategories)
                {
                    // Generate between 5 and 8 products for THIS specific category
                    int productCount = faker.Random.Int(5, 8);

                    for (int i = 0; i < productCount; i++)
                    {
                        var brand = faker.PickRandom(brands);
                        var baseName = faker.PickRandom(blueprint.Value);

                        // Construct a realistic name: "Brand + Item + Color"
                        // Ex: "Nike Air Runner 500 Red"
                        var fullName = $"{brand.Name} {baseName} {faker.Commerce.Color()}";

                        // Unique seed logic: 
                        // Using "CategoryId-Index" ensures images are consistent but distinct per product
                        var seedKey = $"{category.Id}-{i}";

                        var productImages = new List<ProductImage>
                        {
                            new() { IsMain = true, ImageUrl = $"https://picsum.photos/seed/{seedKey}-main/300/300" },
                            new() { IsMain = false, ImageUrl = $"https://picsum.photos/seed/{seedKey}-side/300/300" },
                            new() { IsMain = false, ImageUrl = $"https://picsum.photos/seed/{seedKey}-back/300/300" },
                            new() { IsMain = false, ImageUrl = $"https://picsum.photos/seed/{seedKey}-detail/300/300" }
                        };

                        products.Add(new Product
                        {
                            Name = fullName,
                            Description = faker.Commerce.ProductDescription(),
                            Price = decimal.Parse(faker.Commerce.Price(30, 800)),
                            StockQuantity = faker.Random.Int(10, 100),
                            BrandId = brand.Id,
                            CategoryId = category.Id,
                            IsFeatured = faker.Random.Bool(0.1f), // 10% chance of being featured
                            Images = productImages,
                            Created = DateTime.UtcNow,
                            Updated = DateTime.UtcNow,
                        });
                    }
                }
            }

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }
    }
}