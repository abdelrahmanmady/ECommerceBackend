using ECommerce.Core.Enums;

namespace ECommerce.Core.Entities
{
    public class ProductCareInstruction
    {
        public int Id { get; set; }
        public CareInstructionType Instruction { get; set; }

        //Parent -> Child(ProductCareInstruction)

        //One to Many Relation with Product ( Product (1) -> (M) ProductCareInstruction )
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

    }
}
