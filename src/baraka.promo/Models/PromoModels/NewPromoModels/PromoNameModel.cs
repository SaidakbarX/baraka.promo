using baraka.promo.Data;

namespace baraka.promo.Models.PromoModels.NewPromoModels
{
    public class PromoNameModel
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public PromoView View { get; set; }
        public PromoType Type { get; set; }

    }
}
