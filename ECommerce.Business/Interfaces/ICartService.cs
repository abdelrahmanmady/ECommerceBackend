using ECommerce.Business.DTOs.ShoppingCart.Requests;
using ECommerce.Business.DTOs.ShoppingCart.Responses;

namespace ECommerce.Business.Interfaces
{
    public interface ICartService
    {
        Task<CartResponse> GetCartAsync();
        Task<CartResponse> UpdateCartAsync(UpdateCartRequest updateShoppingCartRequest);
        Task ClearCartAsync();
    }
}
