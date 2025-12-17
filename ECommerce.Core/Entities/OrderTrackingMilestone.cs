using ECommerce.Core.Enums;

namespace ECommerce.Core.Entities
{
    public class OrderTrackingMilestone
    {
        public int Id { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;

        //many to one relation with order
        public int OrderId { get; set; }
        public virtual Order Order { get; set; } = null!;
    }
}
