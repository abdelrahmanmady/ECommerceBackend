namespace ECommerce.Core.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public int HelpfulCount { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        //Parent -> Child(Review)

        //One to Many Relation with ApplicationUser ( ApplicationUser (1) -> (N) Review )
        public string UserId { get; set; } = null!;
        public virtual ApplicationUser User { get; set; } = null!;

        //One to Many Relation with Product ( Product (1) -> (N) Review )
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
