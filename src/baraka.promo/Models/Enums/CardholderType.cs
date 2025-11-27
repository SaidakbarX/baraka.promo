using System.ComponentModel;

namespace baraka.promo.Models.Enums
{
    public enum CardholderType
    {
        [Description("Общий")]
        Common
    }

    public enum CardholderSex: byte
    {
        [Description("Мужской")]
        Male,
        [Description("Женский")]
        Female,
    }
}
