using baraka.promo.Models.Enums;
using baraka.promo.Models.LoyaltyApiModels.FilterModels;

namespace baraka.promo.Models.LoyaltyApiModels.Transactions
{
    public class TransactionInfoModel
    {
        public Guid Id { get; set; }
        public Guid CardId { get; set; }
        public string CardNumber { get; set; }
        public decimal Sum { get; set; }
        public TransactionType Type { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public TransactionStatus Status { get; set; }
        public DateTime? CanceledTime { get; set; }

        public const string CACHE_KEY = "TransactionInfo_CACHE_KEY=";
    }

    public class TransactionRequestModel: PageFilterModel
    {
        public Guid CardId { get; set; }
        public string CardNumber { get; set; }
    }
}
