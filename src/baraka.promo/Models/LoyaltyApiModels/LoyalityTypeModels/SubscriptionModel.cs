namespace baraka.promo.Models.LoyaltyApiModels.LoyalityTypeModels
{
    public class SubscriptionModel
    {
        public int Id { get; set; }
        public string Name { get; set; }         // "Бронзовая карта"
        public string Type { get; set; }         // "bronze", "gold", "platinum"
        public int Quantity { get; set; }      // 100, 200, 300 литров
        public int DiscountPercent { get; set; } // 1, 2, 3 %
        public int DurationMonths { get; set; }  // 12 месяцев (1 год)
        public List<SubscriptionItemPrice> Prices { get; set; }
    }

    public class SubscriptionItemPrice
    {
        public string Type { get; set; } // "Метан", "АИ-92", "АИ-95", "АИ-100"
        public decimal Price { get; set; }   // 3750, 10500, 12800, 17000
    }
}
