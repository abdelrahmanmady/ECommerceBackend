using System.ComponentModel.DataAnnotations;

namespace ECommerce.Business.DTOs.Addresses
{
    public class CreateAddressDto
    {

        //Personal Info
        [MaxLength(50, ErrorMessage = "Full Name must be no longer than 50 characters.")]
        public string FullName { get; set; } = null!;

        [MaxLength(15, ErrorMessage = "Mobile Numer must be no longer than 15 characters.")]
        public string MobileNumber { get; set; } = null!;

        //Address Info
        [MaxLength(60, ErrorMessage = "Street Name must be no longer than 60 characters.")]
        public string Street { get; set; } = null!;

        [MaxLength(50, ErrorMessage = "Building Name/No must be no longer than 60 characters.")]
        public string Building { get; set; } = null!;

        [MaxLength(50, ErrorMessage = "City must be no longer than 50 characters.")]
        public string City { get; set; } = null!;

        [MaxLength(50, ErrorMessage = "District must be no longer than 50 characters.")]
        public string? District { get; set; } = null!;

        [MaxLength(50, ErrorMessage = "Governorate must be no longer than 50 characters.")]
        public string? Governorate { get; set; } = null!;

        [MaxLength(100, ErrorMessage = "Country must be no longer than 100 characters.")]
        public string Country { get; set; } = null!;

        [MaxLength(50, ErrorMessage = "Zip Code must be no longer than 50 characters.")]
        public string? ZipCode { get; set; } = null!;

        [MaxLength(100, ErrorMessage = "Hints must be no longer than 100 characters.")]
        public string? Hints { get; set; }

        [MaxLength(50, ErrorMessage = "Title must be no longer than 50 characters.")]
        public string? Title { get; set; } = null!;
    }
}
