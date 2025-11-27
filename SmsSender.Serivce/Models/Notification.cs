using SmsSender.Serivce.Models.TgMessageModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsSender.Serivce.Models;


public class Notification
{
    [Key]
    public string DeviceId { get; set; }
    [Key]
    public Guid NotificationHeaderId { get; set; }
    public NotificationHeader NotificationHeader { get; set; }
    public Status Status { get; set; }
    public DateTime SendTime { get; set; }
    public Language Language { get; set; }
}

public enum Language : byte
{
    Uz = 1,
    Ru = 2,
    En = 3,
    Kz = 0,
}