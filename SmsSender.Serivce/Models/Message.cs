
using SmsSender.Serivce.Models.TgMessageModel;
using System.ComponentModel.DataAnnotations;

namespace SmsSender.Serivce.Data
{
    public class Message
    {
        [Key]
        public long ChatId { get; private set; }
        public Guid MessageHeaderId { get; private set; }
        public long MessageId { get; set; }
        public Status Status { get; set; }
		public string? Error { get; set; }
        public DateTime SendTime { get; set; }
    }
}
