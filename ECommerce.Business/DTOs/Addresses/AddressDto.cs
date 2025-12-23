namespace ECommerce.Business.DTOs.Addresses
{
    public class AddressDto
    {
        public int Id { get; set; }
        public bool IsDefault { get; set; }

        //Personal Info
        public string FullName { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;

        //Address Info
        public string Street { get; set; } = string.Empty;
        public string Building { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Governorate { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string? ZipCode { get; set; }
        public string? Hints { get; set; }
        public string Title { get; set; } = string.Empty;
    }
}
