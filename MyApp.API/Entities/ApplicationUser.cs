using Microsoft.AspNetCore.Identity;

namespace MyApp.API.Entities
{
    public class ApplicationUser : IdentityUser
    {
        //one to many relation with Orders
        public virtual ICollection<Order> Orders { get; set; } = [];

    }
}
