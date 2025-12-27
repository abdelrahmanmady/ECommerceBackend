namespace ECommerce.Business.DTOs.ShoppingCart.Requests
{
    public class UpdateCartRequest
    {
        public List<UpdateCartItemDto> Items { get; set; } = [];
    }
}
