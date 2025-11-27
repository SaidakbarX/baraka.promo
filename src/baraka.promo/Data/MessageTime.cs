using System.ComponentModel.DataAnnotations;

namespace baraka.promo.Data
{
    public class MessageTime
    {
        [Key]
        public Guid Id { get; set; }
        [Key]
        public Guid MessageHeaderId { get; set; }
        public MessageHeader MessageHeader { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime StopTime { get; set; }
        public string? UserName { get; set; }
    }
}
