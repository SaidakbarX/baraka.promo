using baraka.promo.Data;

namespace baraka.promo
{
    public static class PromoTypeExtensions
    {
        public static string ToStr(this PromoType type)
        {
            switch (type)
            {
                case PromoType.Segment:
                    return "Сегмент";
                case PromoType.Personal:
                    return "Персональный";
                default:
                    return "Все пользователи";
            }
        }

        public static string PromoTypeToStr(PromoType type)
        {
            switch (type)
            {
                case PromoType.Segment:
                    return "Сегмент";
                case PromoType.Personal:
                    return "Персональный";
                default:
                    return "Все пользователи";
            }
        }
    }
}
