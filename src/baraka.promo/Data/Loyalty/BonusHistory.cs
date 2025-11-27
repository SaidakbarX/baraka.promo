namespace baraka.promo.Data.Loyalty
{
    public class BonusHistory
    {
        public Guid TransactionId { get; set; }
        public decimal TotalSum { get; set; }      // общая сумма
        public decimal BonusUsed { get; set; }    // списано бонусами
        public int CashbackId { get; set; }
        public decimal CashbackSum { get; set; }  // начислено кешбек
        public DateTime CreatedAt { get; set; }
    }
}
