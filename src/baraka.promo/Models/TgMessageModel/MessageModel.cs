
namespace baraka.promo.Models.TgMessageModel
{
    public class MessageModel
    {
        public DateTime StartTime { get; set; }
        public DateTime TimeFrom { get; set; }
        public DateTime TimeTo { get; set; }
        public List<string> Phones { get; set; }
        public string Message { get; set; }
        public List<MessageButtonModel>? ButtonInfo { get; set; }
    }

    public class MessageButtonModel
    {
        public string? Text { get; set; }
        public string? Url { get; set; }
    }
}
