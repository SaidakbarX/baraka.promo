using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace baraka.promo.Utils
{
    public static class EnumHelper<T> where T : Enum
    {
        /// <summary>
        /// Gets all names of the enum as a list of strings.
        /// </summary>
        public static List<string> GetEnumNames()
        {
            return Enum.GetNames(typeof(T)).ToList();
        }

        /// <summary>
        /// Gets all descriptions of the enum as a list of strings (fallbacks to names if no description exists).
        /// </summary>
        public static List<string> GetEnumDescriptions()
        {
            return Enum.GetValues(typeof(T))
                       .Cast<T>()
                       .Select(e => GetEnumDescription(e))
                       .ToList();
        }

        /// <summary>
        /// Converts an enum value to its string description (or name if no description is present).
        /// </summary>
        public static string EnumToString(T enumValue)
        {
            return GetEnumDescription(enumValue);
        }

        /// <summary>
        /// Converts a string description back to its corresponding enum value.
        /// </summary>
        public static T StringToEnum(string description)
        {
            foreach (var value in Enum.GetValues(typeof(T)).Cast<T>())
            {
                if (GetEnumDescription(value).Equals(description, StringComparison.OrdinalIgnoreCase))
                {
                    return value;
                }
            }
            throw new ArgumentException($"No enum value found for description '{description}' in {typeof(T).Name}");
        }

        public static List<T> ListToEnumList(List<string> descriptions)
        {
            return descriptions.Select(StringToEnum).ToList();
        }
        /// <summary>
        /// Retrieves the description of an enum value, or its name if no description attribute exists.
        /// </summary>
        private static string GetEnumDescription(T enumValue)
        {
            var fieldInfo = typeof(T).GetField(enumValue.ToString());
            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : enumValue.ToString();
        }
    }
}