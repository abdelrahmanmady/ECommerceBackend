using ECommerce.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Data.Config
{
    public class ProductCareInstructionConfiguration : IEntityTypeConfiguration<ProductCareInstruction>
    {
        public void Configure(EntityTypeBuilder<ProductCareInstruction> builder)
        {
            builder.HasQueryFilter(ci => !ci.Product.IsDeleted);
            builder.Property(ci => ci.Instruction)
                .HasConversion<string>()
                .HasMaxLength(100);
        }
    }
}
