

using Microsoft.AspNetCore.Identity;

namespace ECommerce.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        //one to many relation with Orders
        public virtual ICollection<Order> Orders { get; set; } = [];

        //one to many relation with Addresses
        public virtual ICollection<Address> Addresses { get; set; } = [];

        //one to one relation with ShoppingCart
        public virtual ShoppingCart ShoppingCart { get; set; } = null!;

        //one to many relation with RefreshTokens
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = [];

    }
}
