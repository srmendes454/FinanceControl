using System.ComponentModel;

namespace FinanceControl.Application.Extensions.Enum
{
    public enum Status
    {
        [Description("Aberta")]
        OPEN,

        [Description("Fechada")]
        CLOSED,

        [Description("Disponivel para pagamento")]
        AVAILABLE_FOR_PAYMENT,

        [Description("Atrasada")]
        OVERDUE,

        [Description("Paga")]
        PAID
    }
}