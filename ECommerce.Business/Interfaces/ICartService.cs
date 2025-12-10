using ECommerce.Business.DTOs.ShoppingCart;

namespace ECommerce.Business.Interfaces
{
    public interface ICartService
    {
        Task<ShoppingCartDto> GetAsync();
        Task<ShoppingCartDto> AddItemAsync(int productId);
        Task<ShoppingCartDto> RemoveItemAsync(int productId);
        Task ClearAsync();
    }
}
