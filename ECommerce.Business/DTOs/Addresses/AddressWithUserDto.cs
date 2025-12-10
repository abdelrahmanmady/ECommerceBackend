namespace ECommerce.Business.DTOs.Addresses
{
    public class AddressWithUserDto : AddressDto
    {
        public string UserId { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
