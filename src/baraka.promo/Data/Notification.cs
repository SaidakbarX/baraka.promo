using baraka.promo.Models.Enums;
using baraka.promo.Models.TgMessageModel;
using baraka.promo.Utils;
using System.ComponentModel.DataAnnotations;

namespace baraka.promo.Data;

public class Notification
{
    [Key]
    public string DeviceId { get; set; }
    [Key]
    public Guid NotificationHeaderId { get; set; }
    public NotificationHeader NotificationHeader { get; set; }


    public Status Status { get; set; }
    public DateTime SendTime { get; set; }
    [StringLength(20)]
    public string? Phone { get; set; }
    public DateTime? ReadAt { get; set; }
    public Language Language { get; set; }
}
