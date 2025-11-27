using System.ComponentModel.DataAnnotations;

namespace baraka.promo.Models.PromoModels.NewPromoModels
{
    public class PromoGroupModel
    {
        public int Id { get; set; }
        public int Order { get; set; }
        [MaxLength(128)]
        public string Name { get; set; } = "";
        [MaxLength(512)]
        public string Description { get; set; } = string.Empty;
        public string ModifiedBy { get; set; } = "";

        public int MemberCount { get; set; }
        public int TotalUsedCount { get; set; }
        public int TotalActive { get; set; }
        public int TotalInActive { get; set; }
    }
}
