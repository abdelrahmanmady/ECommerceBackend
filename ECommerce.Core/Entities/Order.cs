using ECommerce.Core.Enums;

namespace ECommerce.Core.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public OrderStatus Status { get; set; }
        public decimal Subtotal { get; set; }
        public decimal ShippingFees { get; set; }
        public decimal Taxes { get; set; }
        public decimal TotalAmount { get; set; }
        public ShippingMethod ShippingMethod { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public OrderAddress ShippingAddress { get; set; } = null!;

        //many to one relation with User
        public string UserId { get; set; } = null!;
        public virtual ApplicationUser User { get; set; } = null!;

        //one to many relation with OrderItems
        public virtual ICollection<OrderItem> Items { get; set; } = [];

        //one to many relation with OrderTrackingMilestones
        public virtual ICollection<OrderTrackingMilestone> OrderTrackingMilestones { get; set; } = [];
    }
}
