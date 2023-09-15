using System.ComponentModel;

namespace FinanceControl.Application.Extensions.Enum
{
    public enum Status
    {
        [Description("Criado")]
        CREATED,

        [Description("A pagar")]
        TO_WIN,

        [Description("Vencido")]
        OVERDUE,

        [Description("Pago")]
        PAID_OUT
    }
}