using Microsoft.EntityFrameworkCore;

namespace ECommerce.Core.Entities
{
    [Owned]
    public class ProductItemOrdered
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string PictureUrl { get; set; } = null!;
    }
}