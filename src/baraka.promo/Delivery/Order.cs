using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace baraka.promo.Delivery
{
    public class Order
    {
        public Guid Id { get; private set; }
        public int Number { get; private set; }
        public DateTime DateTime { get; private set; }
        [ForeignKey("Region")]
        public int RegionId { get; private set; }
        public Region Region { get; private set; }

        [ForeignKey("Restaurant")]
        public Guid? RestaurantId { get; private set; }
        public Restaurant Restaurant { get; private set; }
        public int CountOfItems { get; private set; }
        public decimal SumOfItems { get; private set; }
        public int OrderTypeId { get; private set; }
        public Guid? CustomerId { get; set; }
    }

    public class Delivery
    {
        [Key]
        [ForeignKey("Order")]
        public Guid OrderId { get; private set; }
        public virtual Order Order { get; private set; }
        [StringLength(12)]
        public string Phone1 { get; private set; }
        public string? ContactName { get; private set; }
        public decimal PriceOfDelivery { get; set; }
        public byte Status { get; private set; }
    }
}