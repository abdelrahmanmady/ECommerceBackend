namespace ECommerce.Core.Entities
{
    public class ProductAttribute
    {
        public int Id { get; set; }
        public string Key { get; set; } = null!;
        public string Value { get; set; } = null!;

        //Parent -> Child(ProductAttribute)

        //One to Many Relation with Product ( Product (1) -> (N) ProductAttribute)
        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
    }
}
