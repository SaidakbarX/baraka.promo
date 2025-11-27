using System.Security.Cryptography;

namespace baraka.promo.Models.LoyaltyApiModels.FilterModels
{
    public class FilterModel : PageFilterModel
    {
        public string? SearchText { get; set; }
    }

    public class PromoFilter : PageFilterModel
    {
        public int GroupId { get; set; }
        public string SearchText { get; set; }

        public bool FromPromoList { get; set; }
        public bool IsArchive { get; set; }
        public bool IsPromotion { get; set; }
        public List<string> machanicTypes { get; set; } = new();
        public List<string> promoAudtoria { get; set; } = new();
    }
}
