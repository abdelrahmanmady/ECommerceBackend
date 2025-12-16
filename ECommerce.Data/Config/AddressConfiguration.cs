using ECommerce.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Data.Config
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.Property(a => a.Street).HasColumnType("NVARCHAR(100)");
            builder.Property(a => a.City).HasColumnType("NVARCHAR(100)");
            builder.Property(a => a.State).HasColumnType("NVARCHAR(100)");
            builder.Property(a => a.PostalCode).HasColumnType("NVARCHAR(100)");
            builder.Property(a => a.Country).HasColumnType("NVARCHAR(100)");
        }
    }
}
