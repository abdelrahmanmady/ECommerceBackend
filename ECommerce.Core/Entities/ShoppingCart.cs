namespace ECommerce.Core.Entities
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public DateTime Updated { get; set; }

        //one to one relation with user
        public string UserId { get; set; } = null!;
        public virtual ApplicationUser User { get; set; } = null!;

        //one to many relation with cartItems
        public virtual ICollection<CartItem> Items { get; set; } = [];
    }
}
