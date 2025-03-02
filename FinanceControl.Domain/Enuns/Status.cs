using System.ComponentModel;

namespace FinanceControl.Domain.Enuns
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
        PAID,

        [Description("Paga Parcialmente")]
        PARTIALLY_PAID
    }
}