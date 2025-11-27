
using System.ComponentModel.DataAnnotations.Schema;

namespace baraka.promo.Models.PromoModels
{
    public class ProductCategoryModel
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public ProductCategoryModel? Parent { get; set; }

        public string? Name_Ru { get; set; }
        public string? Name_Uz { get; set; }
        public string? Name_En { get; set; }
    }
}
