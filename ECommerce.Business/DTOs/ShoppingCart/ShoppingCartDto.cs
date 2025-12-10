namespace ECommerce.Business.DTOs.ShoppingCart
{
    public class ShoppingCartDto
    {
        public int Id { get; set; }
        public ICollection<CartItemDto> Items { get; set; } = [];
        public decimal CartTotal => Items.Sum(i => i.Total);
    }
}
