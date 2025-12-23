using ECommerce.Business.DTOs.Addresses;

namespace ECommerce.Business.Interfaces
{
    public interface IAddressService
    {
        Task<IEnumerable<AddressDto>> GetAllAddressesAsync();
        Task<AddressDto> CreateAddressAsync(CreateAddressDto dto);
        Task<AddressDto> UpdateAddressAsync(int addressId, UpdateAddressDto dto);
        Task<IEnumerable<AddressDto>> SetDefaultAsync(int addressId);
        Task DeleteAddressAsync(int addressId);
    }
}
