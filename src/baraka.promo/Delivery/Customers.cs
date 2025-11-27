
using baraka.promo.Models.Enums;

namespace baraka.promo.Delivery
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public string Phone1 { get; set; }
        public int RegionId { get; set; }
        public Language Language { get;  set; }
        public bool IsConfirmed { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedTime { get; set; }
        public string? Phone2 { get; private set; }
    }

    
}
