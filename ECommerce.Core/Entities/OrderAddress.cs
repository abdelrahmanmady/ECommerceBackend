using Microsoft.EntityFrameworkCore;

namespace ECommerce.Core.Entities
{
    [Owned]
    public class OrderAddress
    {

        public int Id { get; set; }

        //Personal Info
        public string FullName { get; set; } = null!;
        public string MobileNumber { get; set; } = null!;

        //Address Info
        public string Street { get; set; } = null!;
        public string Building { get; set; } = null!;
        public string City { get; set; } = null!;
        public string District { get; set; } = null!;
        public string Governorate { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string? ZipCode { get; set; } = null!;
        public string? Hints { get; set; }
        public string? Title { get; set; } = null!;
    }
}
