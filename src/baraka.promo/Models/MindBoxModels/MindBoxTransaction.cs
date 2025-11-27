
namespace baraka.promo.Models.MindBoxModels
{
    public class MindBoxTransaction
    {
        public string pointOfContact { get; set; }
        public CustomerModel customer { get; set; }
        public OrderModel order { get; set; }
    }

    public class BalanceType
    {
        public Ids ids { get; set; }
    }

    public class BonusPoint
    {
        public string amount { get; set; }
        public BalanceType balanceType { get; set; }
    }

    public class Coupon
    {
        public Ids ids { get; set; }
    }

    public class CustomerModel
    {
        public string mobilePhone { get; set; }
        //public Ids ids { get; set; }
    }

    public class CustomFields
    {
        public string deliveryAddress { get; set; }
        public string deliveryDateAndTime { get; set; }
        public string filialDostavki { get; set; }
        public string nPS { get; set; }
        public string orderType { get; set; }
    }

    public class Ids
    {
        //public string mindboxId { get; set; }
        //public string externalClientId { get; set; }
        public string externalOrderId { get; set; }
        //public string systemName { get; set; }
        //public string code { get; set; }
        public string productExternalId { get; set; }
        public string externalId { get; set; }
    }

    public class Line
    {
        public decimal basePricePerItem { get; set; }
        public int quantity { get; set; }
        public int lineNumber { get; set; }
        public Product product { get; set; }
        public Status status { get; set; }
    }

    public class OrderModel
    {
        public decimal totalPrice { get; set; }
        //public string email { get; set; }
        public string mobilePhone { get; set; }
        public Ids ids { get; set; }
        //public CustomFields customFields { get; set; }
        //public List<BonusPoint> bonusPoints { get; set; }
        //public List<Coupon> coupons { get; set; }
        public List<Line> lines { get; set; }
        //public List<Payment> payments { get; set; }
    }

    public class Payment
    {
        public string type { get; set; }
        public string id { get; set; }
        public string amount { get; set; }
    }

    public class Product
    {
        public Ids ids { get; set; }
    }

    public class Status
    {
        public Ids ids { get; set; }
    }

    public class MindBoxTransactionResult
    {
        public string status { get; set; }
        public bool Success => status == "Success";
    }
}
