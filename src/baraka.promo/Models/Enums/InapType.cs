using System.ComponentModel;

namespace baraka.promo.Models.Enums
{
    public enum InapType
    {
        [Description("Не указано")]
        None = 0,

        [Description("Категория ")]
        Category = 1,

        [Description("Продукт")]
        Product = 2,
    }
}
