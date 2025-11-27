using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsSender.Serivce.Models;

public class NotificationHeader
{
    [Key]
    public Guid Id { get; set; }
    [StringLength(4000)]
    public string? MessageUz { get; set; }
    public DateTime CreatedTime { get; set; }
    [StringLength(250)]
    public string? CreatedBy { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime WorkingTimeFrom { get; set; }
    public DateTime WorkingTimeTo { get; set; }
    public int? SegmentId { get; set; }
    public bool? IsImage { get; set; }
    public MessageHeaderStatus Status { get; set; }
    public bool IsDeleted { get; set; }
    [StringLength(4000)]
    public string? MessageRu { get; set; }
    [StringLength(4000)]
    public string? MessageEn { get; set; }
    [StringLength(4000)]
    public string? MessageKz { get; set; }
}