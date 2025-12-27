using ECommerce.Business.DTOs.Addresses.Requests;
using ECommerce.Business.DTOs.Addresses.Responses;

namespace ECommerce.Business.Interfaces
{
    public interface IAddressService
    {
        Task<IEnumerable<AddressSummaryDto>> GetAllAddressesAsync();
        Task<AddressSummaryDto> CreateAddressAsync(CreateAddressRequest dto);
        Task<AddressSummaryDto> UpdateAddressAsync(int addressId, UpdateAddressRequest dto);
        Task<IEnumerable<AddressSummaryDto>> SetDefaultAsync(int addressId);
        Task DeleteAddressAsync(int addressId);
    }
}
