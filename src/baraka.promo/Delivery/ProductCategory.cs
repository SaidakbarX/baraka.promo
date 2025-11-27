using System.ComponentModel.DataAnnotations.Schema;

namespace baraka.promo.Delivery
{
    public class ProductCategory
    {
        public Guid Id { get; private set; }

        [ForeignKey("Parent")]
        public Guid? ParentId { get; private set; }
        public ProductCategory? Parent { get; private set; }

        public string? Name_Ru { get; private set; }
        public string? Name_Uz { get; private set; }
        public string? Name_En { get; private set; }

        public bool IsDeleted { get; private set; }
    }
}