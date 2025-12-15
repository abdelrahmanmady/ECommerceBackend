using ECommerce.Core.Enums;

namespace ECommerce.Business.DTOs.OrderTrackingMilestones
{
    public class OrderTrackingMilestoneDto
    {
        public OrderStatus Status { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
