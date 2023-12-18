using System.ComponentModel;

namespace FinanceControl.Application.Extensions.Enum
{
    public enum Status
    {
        [Description("Aberta")]
        OPEN,

        [Description("Fechada")]
        CLOSED,

        [Description("A pagar")]
        PAYABLE,

        [Description("Atrasada")]
        OVERDUE,

        [Description("Paga")]
        PAID
    }
}