using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyApp.API.Entities;

namespace MyApp.API.Data.Config
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasData(
                new ApplicationUser
                {
                    Id = "user-1",
                    UserName = "john.doe",
                    NormalizedUserName = "JOHN.DOE",
                    Email = "john@example.com",
                    NormalizedEmail = "JOHN@EXAMPLE.COM",
                    EmailConfirmed = true,
                    SecurityStamp = "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
                    ConcurrencyStamp = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb",
                    PasswordHash = "AQAAAAIAAYagAAAAEAHYqA2M9Lpug7ekcTtmsyQ3gRxk85klM6FPzjn4HM3bGvtXdo+Pvpl5UYob6vZ/0Q=="
                },
                new ApplicationUser
                {
                    Id = "user-2",
                    UserName = "mady",
                    NormalizedUserName = "MADY",
                    Email = "mady@example.com",
                    NormalizedEmail = "MADY@EXAMPLE.COM",
                    EmailConfirmed = true,
                    SecurityStamp = "cccccccc-cccc-cccc-cccc-cccccccccccc",
                    ConcurrencyStamp = "dddddddd-dddd-dddd-dddd-dddddddddddd",
                    PasswordHash = "AQAAAAIAAYagAAAAEMHNC5SaiQqJIjZWkV6tC1H92C9/RCeeCYFSaRlUQK94mjhRsaz4+rQIe3POH7FmTA=="
                }
            );
        }
    }
}
