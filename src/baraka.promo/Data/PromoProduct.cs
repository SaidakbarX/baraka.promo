namespace baraka.promo.Data
{
    public class PromoProduct
    {
        public string ProductId { get; set; }
        public long PromoId { get; set; }
        public int? Discount { get; set; }
        /// <summary>
        /// Если PromoView=ProductDiscount - на какое кол-во товара с таким Id в заказе применяется данная скидка.
        /// Значение по умолчанию (1). Если значение ровно (0) ко всем товарам с таким Id в заказе применяется данная скидка
        /// Если PromoView=FreeProduct - кол-во бесплатных товаров
        /// </summary>
        public int Count { get; set; }
        //public PromoConditionType PromoConditionType { get; set; }
    }

    //public enum PromoConditionType
    //{
    //    Itself,
    //    Condition
    //}

}