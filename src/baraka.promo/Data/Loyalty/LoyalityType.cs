using System.ComponentModel.DataAnnotations;

namespace baraka.promo.Data.Loyalty
{
    public class LoyalityType
    {
        public int Id { get; set; }
        [StringLength(250)]
        public string Type { get; set; }
        public string? ValueInfo { get; set; }
        public bool IsActive { get; set; }
    }
}
