using System.ComponentModel.DataAnnotations.Schema;

namespace baraka.promo.Delivery
{
    public class Restaurant
    {
        public Guid Id { get; private set; }
        public string? Name { get; private set; }

        [ForeignKey("Region")]
        public int RegionId { get; private set; }
        public Region Region { get; private set; }
    }
}