namespace ECommerce.Core.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        //ProductDetails
        public string? OverviewHeadline { get; set; }
        public string OverviewDescription { get; set; } = null!;
        public string CompositionText { get; set; } = null!;
        public bool IsSustainable { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsDeleted { get; set; } = false;
        public byte[] Version { get; set; } = [];
        public decimal AverageRating { get; set; }
        public int ReviewsCount { get; set; }

        //Parent -> Product(Child)

        //One to Many Relation with Category ( Category (1) -> (N) Porduct ) 
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;

        //One to Many Relation with Brand ( Brand (1) -> (N) Porduct ) 
        public int BrandId { get; set; }
        public virtual Brand Brand { get; set; } = null!;

        //Product(Parent) -> Child

        //One to Many Relationship with ProductImage ( Product (1) -> (N) ProductImage ) 
        public virtual ICollection<ProductImage> Images { get; set; } = [];

        //One to Many Relation with CartItems ( Product (1) -> (N) CartItem ) 
        public virtual ICollection<CartItem> CartItems { get; set; } = [];

        //One to Many Relation with WishlistItem ( Product (1) -> (N) WishListItem )
        public virtual ICollection<WishlistItem> WishlistItems { get; set; } = [];

        //One to Many Relation with ProductCareInstruction ( Product (1) -> (N) ProductCareInstruction )
        public virtual ICollection<ProductCareInstruction> CareInstructions { get; set; } = [];

        //One to Many Relation with ProductFeature ( Product (1) -> (N) ProductFeature )
        public virtual ICollection<ProductFeature> Features { get; set; } = [];

        //One to Many Relation with ProductAttribute ( Product (1) -> (N) ProductAttribute )
        public virtual ICollection<ProductAttribute> Attributes { get; set; } = [];

        //One to Many Relation with Review ( Product (1) -> (N) Review )
        public virtual ICollection<Review> Reviews { get; set; } = [];
    }
}
