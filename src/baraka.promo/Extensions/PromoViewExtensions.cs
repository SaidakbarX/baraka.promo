using baraka.promo.Data;

namespace baraka.promo
{
    public static class PromoViewExtensions
    {
        public static string ToStr(this PromoView view)
        {
            switch (view)
            {
                case PromoView.FreeDelivery:
                    return "Бесплатная доставка";
                case PromoView.FreeProduct:
                    return "Бесплатное блюдо";
                case PromoView.OrderDiscount:
                    return "Скидка на заказ";
                default:
                    return "Скидка на блюдо";
            }
        }

        public static string PromoViewToStr(PromoView view)
        {
            switch (view)
            {
                case PromoView.FreeDelivery:
                    return "Бесплатная доставка";
                case PromoView.FreeProduct:
                    return "Бесплатное блюдо";
                case PromoView.OrderDiscount:
                    return "Скидка на заказ";
                default:
                    return "Скидка на блюдо";
            }
        }
    }
}
