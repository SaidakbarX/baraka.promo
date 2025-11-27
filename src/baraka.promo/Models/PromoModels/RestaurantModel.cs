using baraka.promo.Delivery;
using System.ComponentModel.DataAnnotations.Schema;

namespace baraka.promo.Models.PromoModels
{
    public class RestaurantModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }

        public int RegionId { get; set; }
        public RegionModel Region { get; set; }
    }
}
