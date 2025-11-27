using baraka.promo.Models.Enums;
using baraka.promo.Models.LoyaltyApiModels.Cards;

namespace baraka.promo.Models.LoyaltyApiModels.Cardholders
{
    public class CardholderInfoModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public DateTime ModifiedTime { get; set; }
        public DateTime CreatedTime { get; set; }
        public string? DateOfBirth { get; set; }
        public CardholderType Type { get; set; }
        public string Email { get; set; }
        public CardholderSex Sex { get; set; }
        public List<CardInfoModel> Cards { get; set; }

        public Guid CardId => Cards?.FirstOrDefault()?.Id ?? Guid.Empty;
        public string CardNumber => Cards?.FirstOrDefault()?.Number ?? "";
        public decimal Balance => Cards?.FirstOrDefault()?.Balance ?? 0;
        public CardType CardType => Cards?.FirstOrDefault()?.Type ?? CardType.Common;

        public const string CACHE_KEY = "Info_CACHE_KEY=";
    }
}
