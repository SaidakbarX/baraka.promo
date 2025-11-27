using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace baraka.promo.Utils
{
    public static class StringHelper
    {

        public static string GenerateCrdNumber()
        {
            var random = new Random();
            string middleDigits = "";

            for (int i = 0; i < 12; i++)
            {
                middleDigits += random.Next(0, 10); // Generate random digits between 0 and 9
            }

            return $"7777{middleDigits}";

        }
        public static string ToDescription<TEnum>(this TEnum EnumValue) where TEnum : struct
        {
            return GetEnumDescription((Enum)(object)EnumValue);
        }
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            if (fi == null) return value.ToString();

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }


        public static string MoneyToDisplay(decimal? price, bool withMinus = false, bool divideTo100 = false)
        {
            if (price == null) return "";
            if (divideTo100) price /= 100;
            return withMinus && price < 0 ? "-" + price.Value.ToString("#,##0 UZS") : price.Value.ToString("#,##0 UZS");
        }

        public static string DateToDisplay(DateTime? dateTime, bool withHour = false)
        {
            if (dateTime == null) return "";
            return dateTime.Value.ToString($"dd-MMMM yyyy {(withHour ? "HH:mm:ss" : "")}", new CultureInfo("ru-RU"));
        }
        public static string DateToDisplay(string? dateTime, bool withHour = false)
        {
            if (dateTime == null) return "";
            return (DateTime.TryParse(dateTime, out DateTime date)) ? DateToDisplay(date, withHour) : "";
        }
        public static string CardNumber(string? card)
        {
            if (card == null) return "";
            if (card.Length >= 16)
                return $"{card[0..4]} {card[4..8]} {card[8..12]} {card[12..16]}";
            else
                return card;
        }

        public static string DateOnlyDisplay(DateTime? dateTime, bool withHour = false)
        {
            if (dateTime == null) return "";
            if (dateTime == DateTime.MinValue) return "---";
            return dateTime.Value.ToString($"dd.MM.yyyy {(withHour ? "HH:mm:ss" : "")}", new CultureInfo("ru-RU"));
        }

    }
}
