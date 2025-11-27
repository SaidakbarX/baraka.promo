namespace baraka.promo.Models.LoyaltyApiModels
{
    public class LoyaltyResultModel
    {
        public Guid Id { get; set; }
    }

    public class LoyaltyBonusResultModel
    {
        public Guid Id { get; set; }
        //public decimal Balance { get; set; }
        public decimal Cashback { get; set; }
    }

    public class LoyaltyCardResultModel
    {
        public Guid UserId { get; set; }
        //public Guid CardId { get; set; }
    }
}
