
namespace baraka.promo.Models.OrderApiModel
{
    public class PreorderModel
    {
        public CustomerModel customer { get; set; }
        public OrderInfoModel order { get; set; }
    }

    public class CustomerModel
    {
        public string? MobilePhone { get; set; }
    }
    public class OrderInfoModel
    {
        public decimal totalCost { get; set; }
        public decimal deliveryCost { get; set; }
        public DiscountInfoModel? totalDiscount { get; set; }
        public DiscountInfoModel? deliveryDiscount { get; set; }
        public OrderCustomFieldsModel? customFields { get; set; }
        public List<OrderItemModel> lines { get; set; }
        public List<OrderPaymentModel>? payments { get; set; }
        public List<CouponModel>? coupons { get; set; }
    }

    public class OrderCustomFieldsModel
    {
        public string? DeliveryAddress { get; set; }
        public string? DeliveryDateAndTime { get; set; }
        public string? FilialDostavki { get; set; }
        public string? OrderType { get; set; }
        public int RegionId { get; set; }
    }

    public class OrderItemModel
    {
        public OrderStatusModel? status { get; set; }
        public OrderProductInfoModel product { get; set; }
        public string lineId { get; set; }
        public int lineNumber { get; set; }
        public float quantity { get; set; }
        public decimal basePricePerItem { get; set; }
        public decimal discountedPriceOfLine { get; set; }
    }

    public class OrderStatusModel
    {
        public StatusIdsModel Ids { get; set; }
        public string? Reason { get; set; }
    }
    public class StatusIdsModel
    {
        public string ExternalId { get; set; }
    }

    public class OrderProductInfoModel
    {
        public ProductIdsModel Ids { get; set; }
    }

    public class ProductIdsModel
    {
        public string ProductExternalId { get; set; }
    }

    public class OrderPaymentModel
    {
        public string? type { get; set; }
    }

    public class DiscountInfoModel
    {
        public long DiscountId { get; set; }
        public decimal DiscountCost { get; set; }
    }

    public class CouponModel
    {
        public string? Code { get; set; }
        public bool IsUsed { get; set; }
        public PromoApiResultModel? Data { get; set; }
        public Dictionary<string, string>? Reason { get; set; }
    }
}
