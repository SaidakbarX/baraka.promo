using baraka.promo.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace baraka.promo.Data.Subscriptions
{
    public class Subscription
    {
        public long Id { get; set; }
        [StringLength(400)]
        public string NameUz { get; set; }
        [StringLength(300)]
        public string? ProductId { get; set; }
        public int MaxCount { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        [StringLength(400)]
        public string? NameEn { get; set; }
        [StringLength(400)]
        public string? NameRu { get; set; }
        public int CountryId { get; set; }

        [StringLength(600)]
        public string? ShortContent_Ru { get; set; }
        [StringLength(600)]
        public string? ShortContent_Uz { get; set; }
        [StringLength(600)]
        public string? ShortContent_En { get; set; }
        public string? Content_Ru { get; set; }
        public string? Content_Uz { get; set; }
        public string? Content_En { get; set; }
        public DateTime CreatedTime { get; set; }
        public InapType InapType { get; set; }

        [StringLength(200)]
        public string? ImageUrlRu { get; set; }
        [StringLength(200)]
        public string? ImageUrlUz { get; set; }
        [StringLength(200)]
        public string? ImageUrlEn { get; set; }
        public long GroupId { get; set; }
        public decimal Price { get; set; }
        [StringLength(50)]
        public string MXIK { get; set; }
        [StringLength(20)]
        public string PackageCode { get; set; }
        public int Vat { get; set; }
        [StringLength(20)]
        public string? UnitCode { get; set; }

        [StringLength(400)]
        public string NamePurchaseUz { get; set; }
        [StringLength(400)]
        public string? NamePurchaseEn { get; set; }
        [StringLength(400)]
        public string? NamePurchaseRu { get; set; }
        [StringLength(600)]
        public string? ShortContent_Purchase_Ru { get; set; }
        [StringLength(600)]
        public string? ShortContent_Purchase_Uz { get; set; }
        [StringLength(600)]
        public string? ShortContent_Purchase_En { get; set; }
        public string? Content_Purchase_Ru { get; set; }
        public string? Content_Purchase_Uz { get; set; }
        public string? Content_Purchase_En { get; set; }

        [StringLength(200)]
        public string? ImageUrl_Purchase_Ru { get; set; }
        [StringLength(200)]
        public string? ImageUrl_Purchase_Uz { get; set; }
        [StringLength(200)]
        public string? ImageUrl_Purchase_En { get; set; }

        public string? PromotionId { get; set; }
    }
}
