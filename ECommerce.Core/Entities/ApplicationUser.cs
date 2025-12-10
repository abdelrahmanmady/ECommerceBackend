

using Microsoft.AspNetCore.Identity;

namespace ECommerce.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        //one to many relation with Orders
        public virtual ICollection<Order> Orders { get; set; } = [];

        //one to many relation with Addresses
        public virtual ICollection<Address> Addresses { get; set; } = [];

        //one to one relation with ShoppingCart
        public virtual ShoppingCart ShoppingCart { get; set; } = null!;

    }
}
