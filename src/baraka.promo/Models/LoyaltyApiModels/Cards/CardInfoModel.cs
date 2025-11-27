using baraka.promo.Models.Enums;

namespace baraka.promo.Models.LoyaltyApiModels.Cards
{
    public class CardInfoModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Number { get; set; }
        public decimal Balance { get; set; }
        public CardType Type { get; set; }
        public long HolderId { get; set; }
        public string? HolderName { get; set; }
        public string? ProductInfo { get; set; }

        public const string CACHE_KEY = "CardInfoModel_CACHE_KEY=";
    }
}
