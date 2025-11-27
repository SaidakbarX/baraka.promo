using System.ComponentModel.DataAnnotations;

namespace baraka.promo.Data;

public class NotificationTime
{
    [Key]
    public Guid Id { get; set; }
    [Key]
    public Guid NotificationHeaderId { get; set; }
    public NotificationHeader NotificationHeader { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime StopTime { get; set; }
    [StringLength(250)]
    public string? UserName { get; set; }
}
