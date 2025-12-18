using ECommerce.Business.DTOs.ShoppingCart;

namespace ECommerce.Business.Interfaces
{
    public interface ICartService
    {
        Task<ShoppingCartDto> GetAsync();
        Task<ShoppingCartDto> UpdateAsync(UpdateShoppingCartDto dto);
        Task ClearAsync();
    }
}
