using ECommerce.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Data.Config
{
    public class ShoppingCartConfiguration : IEntityTypeConfiguration<ShoppingCart>
    {
        public void Configure(EntityTypeBuilder<ShoppingCart> builder)
        {
            //one to many relation with CartItems
            builder.HasMany(sc => sc.Items)
                .WithOne(i => i.ShoppingCart)
                .HasForeignKey(i => i.ShoppingCartId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
