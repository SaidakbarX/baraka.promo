using System.ComponentModel.DataAnnotations.Schema;

namespace baraka.promo.Delivery
{
    public class Product
    {
        public Guid Id { get; set; }
        public string? Name_Ru { get; private set; }
        public string? Name_Uz { get; private set; }
        public string? Name_En { get; private set; }

        [ForeignKey("Category")]
        public Guid CategoryId { get; private set; }
        public ProductCategory Category { get; private set; }

        [ForeignKey("SubCategory")]
        public Guid? SubCategoryId { get; private set; }
        public ProductCategory? SubCategory { get; private set; }

        public bool IsDeleted { get; private set; }
    }
}