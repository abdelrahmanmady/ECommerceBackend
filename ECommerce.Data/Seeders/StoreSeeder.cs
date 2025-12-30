using Bogus;
using ECommerce.Core.Entities;
using ECommerce.Core.Enums;
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
            // Load categories. 
            // If you have a 'Children' navigation property, .Include(c => c.Children) would be better,
            // but this logic works purely on the flat list of IDs.
            var categories = await context.Categories.ToListAsync();

            var faker = new Faker();
            var products = new List<Product>();

            // 1. Identify Parent Categories (Nodes that are NOT terminal)
            // Any category that is listed as a 'Parent' for someone else is NOT a leaf.
            // We assume your Category entity has a 'ParentId' property. 
            // If it only has a 'Parent' navigation object, let me know.
            var parentCategoryIds = categories
                .Where(c => c.ParentId.HasValue)
                .Select(c => c.ParentId!.Value)
                .Distinct()
                .ToHashSet();

            // 2. Helper to generate relevant attributes dynamically
            List<string> GetAttributeKeys(string categoryName)
            {
                var name = categoryName.ToLower();
                if (name.Contains("shoe") || name.Contains("sneaker") || name.Contains("boot"))
                    return new List<string> { "Size", "Sole Material", "Upper Material" };

                if (name.Contains("watch") || name.Contains("jewelry"))
                    return new List<string> { "Strap Material", "Water Resistance", "Dial Color" };

                if (name.Contains("bag") || name.Contains("tote"))
                    return new List<string> { "Dimensions", "Number of Pockets", "Strap Length" };

                return new List<string> { "Size", "Fit", "Fabric Weight" };
            }

            // 3. Loop through ALL categories
            foreach (var category in categories)
            {
                // FILTER: Skip if this category is a Parent (i.e., NOT terminal)
                if (parentCategoryIds.Contains(category.Id))
                    continue;

                // Generate 3 to 6 products per terminal category
                int productCount = faker.Random.Int(3, 6);

                for (int i = 0; i < productCount; i++)
                {
                    var brand = faker.PickRandom(brands);
                    var fullName = $"{brand.Name} {category.Name} {faker.Commerce.Color()}";
                    var seedKey = $"{category.Id}-{i}";

                    var product = new Product
                    {
                        Name = fullName,
                        Price = decimal.Parse(faker.Commerce.Price(20, 500)),
                        StockQuantity = faker.Random.Int(5, 100),
                        BrandId = brand.Id,
                        CategoryId = category.Id,

                        // Details
                        OverviewHeadline = $"{faker.Commerce.ProductAdjective()} Design, {faker.Commerce.ProductAdjective()} Feel",
                        OverviewDescription = faker.Lorem.Paragraphs(2),
                        CompositionText = faker.PickRandom(new[] { "100% Cotton", "95% Polyester, 5% Elastane", "Genuine Leather", "Recycled Wool" }),

                        // Badges
                        IsSustainable = faker.Random.Bool(0.3f),
                        IsFeatured = faker.Random.Bool(0.1f),
                        IsDeleted = false,

                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow,

                        // Images
                        Images = new List<ProductImage>
                {
                    new() { IsMain = true, ImageUrl = $"https://picsum.photos/seed/{seedKey}-main/400/400" },
                    new() { IsMain = false, ImageUrl = $"https://picsum.photos/seed/{seedKey}-side/400/400" },
                    new() { IsMain = false, ImageUrl = $"https://picsum.photos/seed/{seedKey}-detail/400/400" }
                }
                    };

                    // Features (Bullet points)
                    int featureCount = faker.Random.Int(2, 4);
                    for (int f = 0; f < featureCount; f++)
                    {
                        product.Features.Add(new ProductFeature
                        {
                            Feature = $"{faker.Commerce.ProductAdjective()} {faker.Commerce.ProductMaterial()}"
                        });
                    }

                    // Care Instructions (Enum)
                    var instructions = GetRandomCareInstructions(faker);
                    foreach (var instr in instructions)
                    {
                        product.CareInstructions.Add(new ProductCareInstruction
                        {
                            Instruction = instr,
                        });
                    }

                    // Attributes
                    var attrKeys = GetAttributeKeys(category.Name);
                    foreach (var key in attrKeys)
                    {
                        product.Attributes.Add(new ProductAttribute
                        {
                            Key = key,
                            Value = key == "Size" ? faker.PickRandom("S", "M", "L", "XL") : faker.Commerce.ProductAdjective()
                        });
                    }

                    products.Add(product);
                }
            }

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }

        // Helper: Pick 3 random Enum values
        private static List<CareInstructionType> GetRandomCareInstructions(Faker faker)
        {
            var values = Enum.GetValues<CareInstructionType>()
                                .Cast<CareInstructionType>()
                                .ToList();
            return [.. faker.PickRandom(values, 3)];
        }
    }
}