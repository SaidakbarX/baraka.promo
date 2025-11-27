namespace SmsSender.Serivce
{
    public class MessageModel
    {
        public long MessageId { get; set; }
        public long ChatId { get; set; }
        public Guid MessageHeaderId { get; set; }
        public string? Message { get; set; }
        public bool? IsPhoto { get; set; }
        public string? FileId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Models.TgMessageModel.Status Status { get; set; }
        public string? Error { get; set; }

    }
}
