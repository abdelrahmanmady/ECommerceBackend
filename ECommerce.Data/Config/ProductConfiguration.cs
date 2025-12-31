using ECommerce.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Data.Config
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(p => p.Name).HasMaxLength(200);

            builder.Property(p => p.Price).HasPrecision(18, 2);

            builder.Property(p => p.OverviewHeadline).HasMaxLength(100);

            builder.Property(p => p.OverviewDescription).HasMaxLength(1000);

            builder.Property(p => p.CompositionText).HasMaxLength(100);

            builder.Property(p => p.Version).IsRowVersion();

            builder.Property(p => p.AverageRating).HasPrecision(2, 1);

            builder.HasQueryFilter(p => !p.IsDeleted);

            //One to Many Relationship with ProductImage ( Product (1) -> (N) ProductImage )
            builder.HasMany(p => p.Images)
                .WithOne(i => i.Product)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            //One to Many relation with CartItems ( Product (1) -> (N) CartItem )
            builder.HasMany(p => p.CartItems)
                .WithOne(ci => ci.Product)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            //One To Many Relation with WishlistItem ( Product (1) -> (N) WishListItem )
            builder.HasMany(p => p.WishlistItems)
                .WithOne(wi => wi.Product)
                .HasForeignKey(wi => wi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            //One To Many Relation with ProductCareInstruction ( Product (1) -> (N) ProductCareInstruction )
            builder.HasMany(p => p.CareInstructions)
                .WithOne(ci => ci.Product)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            //One To Many Relation with ProductFeature ( Product (1) -> (N) ProductFeature )
            builder.HasMany(p => p.Features)
                .WithOne(f => f.Product)
                .HasForeignKey(f => f.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            //One To Many Relation with ProductAttribute ( Product (1) -> (N) ProductAttribute )
            builder.HasMany(p => p.Attributes)
                .WithOne(a => a.Product)
                .HasForeignKey(a => a.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            //One to Many Relation with Review ( Product (1) -> (N) Review )
            builder.HasMany(p => p.Reviews)
                .WithOne(r => r.Product)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
