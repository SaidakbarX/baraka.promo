
namespace baraka.promo.Models.LoyaltyApiModels.Transactions
{
    public class TransactionModel
    {
        public string CardNumber { get; set; }
        public decimal Sum { get; set; }
        public string ExternalId { get; set; }
        public string? ExternalData { get; set; }
    }

    public class ReplenishmentModel
    {
        public string CardNumber { get; set; }
        public decimal Sum { get; set; }
    }

    public class TransactionBonusModel
    {
        public string Card { get; set; }
        public decimal Sum { get; set; }
        public decimal BonusSum { get; set; }
        public string ExternalId { get; set; }
        public string? ExternalData { get; set; }
    }

    public class TransactionBonusInfoModel
    {
        public Guid Id { get; set; }
        public Guid CardId { get; set; }
        public decimal Sum { get; set; }
        public decimal CashbackSum { get; set; }
        public string Status { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? CanceledTime { get; set; }

        public const string CACHE_KEY = "BonusTransactionInfo_CACHE_KEY=";
    }
}
