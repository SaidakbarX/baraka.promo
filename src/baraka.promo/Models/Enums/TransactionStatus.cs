using System.ComponentModel;

namespace baraka.promo.Models.Enums
{
    public enum TransactionStatus
    {
        [Description("Успешный")]
        Success,
        [Description("Неуспешный")]
        Failed,
        [Description("Отменено")]
        Cancelled,
        Draft,
    }
}
