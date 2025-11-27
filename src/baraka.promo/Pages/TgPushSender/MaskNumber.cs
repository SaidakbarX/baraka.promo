namespace baraka.promo.Pages.TgPushSender.Utilities
{
    public static class MaskNumber
    {
        public static string Phone(string value)
        {
            //+977 73 *** 3
            if (!string.IsNullOrWhiteSpace(value))
            {
                value = value.Replace(" ", "").Replace("+", "");
                if (value.Length >= 9)
                {
                    if (value.Length >= 12)
                    {
                        return $"+{value.Substring(3, 2)} {value.Substring(5, 3)} *** {value[^1]}";

                        //result.Append('+');
                        //result.Append(value.Substring(3, 2));
                        //result.Append(' ');
                        //result.Append(value.Substring(5, 3));
                        //result.Append(" *** ");
                        //result.Append(value[^1]);
                    }
                    else
                    {
                        return $"+{value.Substring(0, 2)} {value.Substring(2, 3)} *** {value[^1]}";

                        //result.Append('+');
                        //result.Append(value.Substring(0, 2));
                        //result.Append(' ');
                        //result.Append(value.Substring(2, 3));
                        //result.Append(" *** ");
                        //result.Append(value[^1]);
                    }
                } 
            }
            return value;
        }

        public static string PhoneWithSpace(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                value = value.Replace(" ", "").Replace("+", "");
                if (value.Length >= 9)
                {
                    if (value.Length >= 12)
                    {
                        return $"+{value.Substring(0, 3)} {value.Substring(3, 2)} {value.Substring(5, 3)} {value.Substring(8, 2)} {value.Substring(10)}";
                    }
                    else
                    {
                        return $"{value.Substring(0, 2)} {value.Substring(2, 3)} {value.Substring(5, 2)} {value.Substring(7)}";
                    }
                }
            }
            return value;
        }
    }
}