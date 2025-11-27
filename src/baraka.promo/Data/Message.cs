using baraka.promo.Models.TgMessageModel;
using System.ComponentModel.DataAnnotations;

namespace baraka.promo.Data
{
    public class Message
    {
        [Key]
        public long ChatId { get; set; }
        [Key]
        public Guid MessageHeaderId { get; set; }
        public MessageHeader MessageHeader { get; set; }
        public long MessageId { get; set; }
        public Status Status { get; set; }
        public string? Error { get; set; }
        public DateTime SendTime { get; set; }
        [StringLength(20)]
        public string? Phone { get; set; }
    }
}
