namespace ECommerce.Core.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string HierarchyPath { get; set; } = null!;

        //one to many relation with Product
        public virtual ICollection<Product> Products { get; set; } = [];

        //many to one self relation
        public int? ParentId { get; set; }
        public virtual Category? Parent { get; set; }

    }
}
