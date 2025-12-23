namespace ECommerce.Core.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public OrderedProduct OrderedProduct { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal TotalPrice => OrderedProduct.Price * Quantity;

        //many to one relation with Order
        public int OrderId { get; set; }
        public virtual Order Order { get; set; } = null!;

    }
}
