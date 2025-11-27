using System.ComponentModel.DataAnnotations;

namespace baraka.promo.Data
{
    public class Integration
    {
        public int Id { get; set; }
        [StringLength(150)]
        public string Name { get; set; }
        [StringLength(50)]
        public string SecretKey { get; set; }
        public bool IsActive { get; set; }
    }
}
