using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace baraka.promo.Delivery
{
    public class CustomerDevice
    {
        [Key]
        public string DeviceId { get; private set; }
        public DeviceType Type { get; private set; }

        [ForeignKey("Customer")]
        public Guid CustomerId { get; private set; }
        public Customer Customer { get; private set; }
        public DateTime CreatedTime { get; protected set; }
    }

    public enum DeviceType : byte
    {
        IOS = 1,
        Android = 2,
    }
}
