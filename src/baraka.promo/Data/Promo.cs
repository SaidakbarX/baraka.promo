using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace baraka.promo.Data
{
    public class Promo
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int? MaxCount { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public long? MinOrderAmount { get; set; }
        public long? MaxOrderAmount { get; set; }
        public int? OrderDiscount { get; set; }
        public PromoView View { get; set; }
        public PromoType Type { get; set; }
        public int? SegmentId { get; set; }
        public int? TotalCount { get; set; }
        public int? GroupId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsUnique { get; set; }
        public bool IsPromotion { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PromoType
    {
        [Description("Все")]
        All = 0,
        [Description("Персональный")]
        Personal = 1,
        [Description("Сегмент")]
        Segment = 2,
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PromoView
    {
        [Description("Бесплатная доставка")]
        FreeDelivery = 0,
        [Description("Бесплатный продукт")]
        FreeProduct = 1,
        [Description("Скидка на заказ")]
        OrderDiscount = 2,
        [Description("Скидка на продукт")]
        ProductDiscount = 3,
        [Description("Акционное блюда")]
        PromotionalProduct = 4,
        [Description("Скидка на заказ (сум)")]
        OrderDiscountAmount = 5,
        [Description("Минимальная сумма для акционного товара")]
        OrderMinSumPromotion = 6
    }
}