using System.ComponentModel.DataAnnotations;

namespace baraka.promo.Data
{
    public class PromoChildValue
    {
        public long Id { get; set; }

        [StringLength(200)]
        public string Name { get; set; }
        public long PromoId { get; set; }

        [StringLength(50)]
        public string? ClientPhone { get; set; }
        public DateTime? TimeOfUse { get; set; }
    }
}
