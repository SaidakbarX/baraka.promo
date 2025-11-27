namespace baraka.promo.Data
{
    public class PromoClient
    {
        public Guid Id { get; set; }
        public long PromoId { get; set; }
        public string Phone { get; set; }
        public DateTime? TimeOfUse { get; set; }
        public string? OrderId { get; set; }
    }
}