
namespace baraka.promo.Models.PromoModels
{
    public class ProductModel
    {
        public Guid Id { get; set; }
        public string? Name_Ru { get; set; }
        public string? Name_Uz { get; set; }
        public string? Name_En { get; set; }

        public Guid CategoryId { get; set; }
        public ProductCategoryModel Category { get; set; }

        public Guid? SubCategoryId { get; set; }
        public ProductCategoryModel? SubCategory { get; set; }
    }
}
