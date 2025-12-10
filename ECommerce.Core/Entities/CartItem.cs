namespace ECommerce.Core.Entities
{
    public class CartItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }

        //many to one relation with ShoppingCart
        public int ShoppingCartId { get; set; }
        public virtual ShoppingCart ShoppingCart { get; set; } = null!;

        // many to one relation with Product
        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
    }
}
