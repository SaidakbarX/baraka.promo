
using baraka.promo.Models.Enums;

namespace baraka.promo.Models.LoyaltyApiModels.LoyalityTypeModels
{
    public class CashbackModel
    {
        public CardType CardType { get; set; }
        public decimal Value { get; set; }
        public LanguageInfo Oferta { get; set; } = new();
        public string? ImgUrlBlack { get; set; }
        public string? ImgUrlLight { get; set; }
    }

    public class LanguageInfo
    {
        public string? OfertaUz { get; set; }
        public string? OfertaRu { get; set; }
        public string? OfertaEn { get; set; }
    }
}
