namespace ECommerce.Core.Entities
{
    public class ProductFeature
    {
        public int Id { get; set; }
        public string Feature { get; set; } = null!;

        //Parent -> Child(ProductFeature)

        //One to Many Relation with Product ( Product (1) -> (N) ProductFeature )
        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
    }
}
