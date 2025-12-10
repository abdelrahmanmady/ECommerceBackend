namespace ECommerce.Business.DTOs.Addresses
{
    public class UpdateAddressDto
    {
        public string Street { get; set; } = null!;
        public string City { get; set; } = null!;
        public string State { get; set; } = null!;
        public string? PostalCode { get; set; }
        public string Country { get; set; } = null!;
    }
}
