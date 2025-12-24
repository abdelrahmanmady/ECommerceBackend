using Bogus;
using ECommerce.Core.Entities;

namespace ECommerce.Data.Seeders.Fakers
{
    public static class AddressFaker
    {
        public static Faker<Address> GetAddress()
        {
            return new Faker<Address>()
                .RuleFor(a => a.FullName, f => f.Name.FullName())
                .RuleFor(a => a.MobileNumber, f => f.Phone.PhoneNumber("##########"))
                .RuleFor(a => a.Label, f => f.PickRandom("Home", "Work", "Office", "Parents' House"))
                .RuleFor(a => a.Street, f => f.Address.StreetAddress())
                .RuleFor(a => a.Building, f => f.Address.BuildingNumber())
                .RuleFor(a => a.City, f => f.Address.City())
                .RuleFor(a => a.District, (f, a) => $"District of {a.City}")
                .RuleFor(a => a.Governorate, (f, a) => a.City)
                .RuleFor(a => a.Country, f => f.Address.Country())
                .RuleFor(a => a.ZipCode, f => f.Address.ZipCode());

        }
    }
}