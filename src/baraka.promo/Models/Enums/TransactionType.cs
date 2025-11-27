using System.ComponentModel;

namespace baraka.promo.Models.Enums
{
    public enum TransactionType
    {
        [Description("Списания")]
        WriteOff = 1,
        [Description("Пополнение")]
        Replenishment = 2,
        Invoice = 3,
    }
}
