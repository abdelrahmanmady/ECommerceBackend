using ECommerce.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Data.Config
{
    public class ProductAttributeConfiguration : IEntityTypeConfiguration<ProductAttribute>
    {
        public void Configure(EntityTypeBuilder<ProductAttribute> builder)
        {
            builder.HasQueryFilter(a => !a.Product.IsDeleted);
            builder.Property(a => a.Key).HasMaxLength(50);
            builder.Property(a => a.Value).HasMaxLength(100);
        }
    }
}
