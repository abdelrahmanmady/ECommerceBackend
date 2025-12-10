using ECommerce.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Data.Config
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(o => o.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(o => o.TotalAmount)
                .HasColumnType("DECIMAL(18,2)");

            //one to many relation with OrderItems
            builder.HasMany(o => o.Items)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            //configure owned type (snapshot)
            builder.OwnsOne(o => o.ShippingAddress, a =>
            {
                a.WithOwner();
                a.Property(x => x.Street).HasColumnName("ShippingStreet").HasMaxLength(100);
                a.Property(x => x.City).HasColumnName("ShippingCity").HasMaxLength(50);
                a.Property(x => x.State).HasColumnName("ShippingState").HasMaxLength(50);
                a.Property(x => x.PostalCode).HasColumnName("ShippingZipCode").HasMaxLength(20);
                a.Property(x => x.Country).HasColumnName("ShippingCountry").HasMaxLength(50);
            });
        }
    }
}
