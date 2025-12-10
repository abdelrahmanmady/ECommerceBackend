using ECommerce.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Data.Config
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.Property(a => a.Street).HasMaxLength(100);
            builder.Property(a => a.City).HasMaxLength(50);
            builder.Property(a => a.State).HasMaxLength(50);
            builder.Property(a => a.PostalCode).HasMaxLength(20);
            builder.Property(a => a.Country).HasMaxLength(50);
        }
    }
}
