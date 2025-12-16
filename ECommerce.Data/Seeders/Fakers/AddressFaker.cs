using Bogus;
using ECommerce.Core.Entities;

namespace ECommerce.Data.Seeders.Fakers
{
    public static class AddressFaker
    {
        public static Faker<Address> GetAddress()
        {
            return new Faker<Address>()
                .RuleFor(a => a.Street, f => f.Address.StreetAddress())
                .RuleFor(a => a.City, f => f.Address.City())
                .RuleFor(a => a.State, f => f.Address.State())
                .RuleFor(a => a.PostalCode, f => f.Address.ZipCode().Substring(0, 5)) // Limit length for DB column
                .RuleFor(a => a.Country, f => f.Address.Country());
        }
    }
}