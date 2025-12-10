using ECommerce.Business.DTOs.Addresses;

namespace ECommerce.Business.Interfaces
{
    public interface IAddressService
    {
        Task<IEnumerable<AddressDto>> GetAllAsync();
        Task<AddressDto> GetByIdAsync(int id);
        Task<AddressDto> CreateAsync(CreateAddressDto dto);
        Task<AddressDto> UpdateAsync(int id, UpdateAddressDto dto);
        Task DeleteAsync(int id);
    }
}
