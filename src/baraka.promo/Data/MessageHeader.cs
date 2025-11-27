using baraka.promo.Models;
using System.ComponentModel.DataAnnotations;

namespace baraka.promo.Data
{
    public class MessageHeader
    {
        [Key]
        public Guid Id { get; set; }
        [StringLength(4000)]
        public string? Message { get; set; }
        public DateTime CreatedTime { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime WorkingTimeFrom { get; set; }
        public DateTime WorkingTimeTo { get; set; }
        public int? SegmentId { get; set; }
        public string? Json { get; set; }
        public string? FileId { get; set; }
        public bool? IsImage { get; set; }
        public string? JsonButtons { get; set; }
        public MessageHeaderStatus Status { get; set; }
        public bool IsDeleted { get; set; }

    }
}
